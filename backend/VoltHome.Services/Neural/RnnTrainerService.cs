using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore; 
using VoltHome.Infrastructure;
using VoltHome.Services.Neural.Model;

namespace VoltHome.Services.Neural;

public class RnnTrainerService
{
    private readonly AppDbContext _context;
    private readonly MLContext _ml;

    public RnnTrainerService(AppDbContext context)
    {
        _context = context;
        _ml = new MLContext(seed: 1);
    }

    public async Task TrainAsync(CancellationToken ct)
    {
        var data = await _context.SolarStationEnergySnapshots
            .OrderBy(x => x.SolarStationId)
            .ThenBy(x => x.HourStartUtc)
            .ToListAsync(ct);

        var dataset = new List<RnnInputModel>();

        foreach (var group in data.GroupBy(x => x.SolarStationId))
        {
            var ordered = group.OrderBy(x => x.HourStartUtc).ToList();

            if (ordered.Count < 5)
                continue;

            for (int i = 3; i < ordered.Count; i++)
            {
                dataset.Add(new RnnInputModel
                {
                    Sequence = new float[]
                    {
                    (float)ordered[i - 3].GeneratedKwh,
                    (float)ordered[i - 2].GeneratedKwh,
                    (float)ordered[i - 1].GeneratedKwh
                    },
                    Label = (float)ordered[i].GeneratedKwh
                });
            }
        }

        if (dataset.Count == 0)
            throw new Exception("No data");

        var mlData = _ml.Data.LoadFromEnumerable(dataset);

        var pipeline =
            _ml.Transforms
                .Concatenate("Features", nameof(RnnInputModel.Sequence))
                .Append(_ml.Regression.Trainers.FastTree());

        var model = pipeline.Fit(mlData);

        var path = Path.Combine(AppContext.BaseDirectory, "Models");
        Directory.CreateDirectory(path);

        _ml.Model.Save(model, mlData.Schema, Path.Combine(path, "rnn-model.zip"));

        var engine = _ml.Model.CreatePredictionEngine<RnnInputModel, RnnOutputModel>(model);

        Console.WriteLine("\n==============================");
        Console.WriteLine(" RNN TIME SERIES ANALYSIS");
        Console.WriteLine("==============================\n");

        Console.WriteLine("IDX | ACTUAL | PREDICT | ERROR");

        float totalError = 0;

        for (int i = 0; i < dataset.Count; i++)
        {
            var pred = engine.Predict(dataset[i]).Score;
            var actual = dataset[i].Label;
            var error = Math.Abs(actual - pred);

            totalError += error;

            Console.WriteLine(
                $"{i,3} | {actual,6:F2} | {pred,7:F2} | {error,6:F2}");
        }

        Console.WriteLine("\n------------------------------");
        Console.WriteLine($"AVG ERROR: {totalError / dataset.Count:F4}");
        Console.WriteLine("------------------------------\n");

        Console.WriteLine(" ERROR VISUALIZATION:\n");

        for (int i = 0; i < dataset.Count; i++)
        {
            var pred = engine.Predict(dataset[i]).Score;
            var error = Math.Abs(dataset[i].Label - pred);

            int bars = (int)(error * 3);

            Console.WriteLine($"{i,3} | " + new string('█', bars));
        }

        Console.WriteLine("\n TIME SERIES (ACTUAL vs PREDICTED)\n");

        for (int i = 0; i < dataset.Count; i++)
        {
            var pred = engine.Predict(dataset[i]).Score;
            var actual = dataset[i].Label;

            int diffBars = (int)Math.Abs(actual - pred);

            Console.WriteLine(
                $"{i,3}  A:{actual,5:F1}  P:{pred,5:F1}  " +
                new string('*', diffBars));
        }

        Console.WriteLine("\n==============================");
        Console.WriteLine(" DEBUG COMPLETE");
        Console.WriteLine("==============================\n");
    }
}
