using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.dbo;
using VoltHome.Infrastructure;
using VoltHome.Services.Neural.Model;

namespace VoltHome.Services.Neural;

public class NeuralTrainerService
{
    private readonly AppDbContext _context;
    private readonly MLContext _ml;

    public NeuralTrainerService(AppDbContext context)
    {
        _context = context;
        _ml = new MLContext(seed: 1);
    }

    public async Task TrainAsync(CancellationToken ct)
    {
        var dataRows = await _context.SolarStationEnergySnapshots
            .OrderBy(x => x.SolarStationId)
            .ThenBy(x => x.HourStartUtc)
            .ToListAsync(ct);

        var dataset = new List<SimpleSolarModel>();

        foreach (var group in dataRows.GroupBy(x => x.SolarStationId))
        {
            var ordered = group.OrderBy(x => x.HourStartUtc).ToList();

            for (int i = 1; i < ordered.Count; i++)
            {
                dataset.Add(new SimpleSolarModel
                {
                    PrevHour = (float)ordered[i - 1].GeneratedKwh,
                    Label = (float)ordered[i].GeneratedKwh
                });
            }
        }

        if (dataset.Count < 10)
            throw new Exception("Not enough data");

        Console.WriteLine("\n==============================");
        Console.WriteLine(" NEURAL TRAINING ");
        Console.WriteLine("==============================\n");


        var baseMl = new MLContext(seed: 1);
        var baseData = baseMl.Data.LoadFromEnumerable(dataset);

        var baseModel = baseMl.Transforms
            .Concatenate("Features", nameof(SimpleSolarModel.PrevHour))
            .Append(baseMl.Regression.Trainers.FastTree())
            .Fit(baseData);

        var baseEngine =
            baseMl.Model.CreatePredictionEngine<SimpleSolarModel, SimpleSolarModel>(baseModel);

        float baseLoss = CalculateLoss(baseEngine, dataset);

        Console.WriteLine($" BASELINE LOSS: {baseLoss:F4}");

        var configs = new[]
        {
        new { Trees = 50, Label = "LOW CAPACITY" },
        new { Trees = 100, Label = "DEFAULT" },
        new { Trees = 200, Label = "HIGH CAPACITY" }
    };

        var results = new List<(string name, float loss)>();

        foreach (var cfg in configs)
        {
            Console.WriteLine($"\n RUN: {cfg.Label} (trees={cfg.Trees})");

            var ml = new MLContext(seed: cfg.Trees);

            var data = ml.Data.LoadFromEnumerable(dataset);

            var pipeline =
                ml.Transforms.Concatenate("Features", nameof(SimpleSolarModel.PrevHour))
                .Append(ml.Regression.Trainers.FastTree(
                    numberOfLeaves: 20,
                    numberOfTrees: cfg.Trees,
                    minimumExampleCountPerLeaf: 10));

            var model = pipeline.Fit(data);

            var engine = ml.Model.CreatePredictionEngine<SimpleSolarModel, SimpleSolarModel>(model);

            float loss = CalculateLoss(engine, dataset);

            results.Add((cfg.Label, loss));

            Console.WriteLine($" LOSS: {loss:F4}");

            Console.WriteLine("SAMPLE PREDICTIONS:");
            for (int i = 0; i < Math.Min(5, dataset.Count); i++)
            {
                var p = engine.Predict(dataset[i]).PrevHour;
                Console.WriteLine($"  y={dataset[i].Label:F2} → ŷ={p:F2}");
            }
        }

        Console.WriteLine("\n==============================");
        Console.WriteLine(" MODEL COMPARISON");
        Console.WriteLine("==============================\n");

        foreach (var r in results)
        {
            int bars = (int)(r.loss * 10);

            Console.WriteLine($"{r.name,-12} LOSS={r.loss:F4} | {new string('█', bars)}");
        }

        var best = results.OrderBy(x => x.loss).First();

        Console.WriteLine($"\n🏆 BEST MODEL: {best.name}");

        var finalMl = new MLContext(seed: 42);
        var finalData = finalMl.Data.LoadFromEnumerable(dataset);

        var finalModel = finalMl.Transforms
            .Concatenate("Features", nameof(SimpleSolarModel.PrevHour))
            .Append(finalMl.Regression.Trainers.FastTree(numberOfTrees: 150))
            .Fit(finalData);

        var finalEngine =
            finalMl.Model.CreatePredictionEngine<SimpleSolarModel, SimpleSolarModel>(finalModel);

        Console.WriteLine("\n==============================");
        Console.WriteLine(" TIME SERIES DEBUG (FINAL MODEL)");
        Console.WriteLine("==============================\n");

        float total = 0;

        for (int i = 0; i < dataset.Count; i++)
        {
            var actual = dataset[i].Label;
            var pred = finalEngine.Predict(dataset[i]).PrevHour;
            var err = Math.Abs(actual - pred);

            total += err;

            Console.WriteLine($"{i,3} | A:{actual,5:F2} P:{pred,5:F2} E:{err,5:F2}");
        }

        Console.WriteLine($"\nAVG ERROR: {total / dataset.Count:F4}");

        Console.WriteLine("\n📉 ERROR GRAPH:");

        for (int i = 0; i < dataset.Count; i++)
        {
            var p = finalEngine.Predict(dataset[i]).PrevHour;
            var e = Math.Abs(dataset[i].Label - p);

            Console.WriteLine($"{i,3} | {new string('█', (int)(e * 3))}");
        }

        Console.WriteLine("\n==============================");
        Console.WriteLine(" TRAINING COMPLETE");
        Console.WriteLine("==============================");
    }

    private float CalculateLoss(PredictionEngine<SimpleSolarModel, SimpleSolarModel> engine, List<SimpleSolarModel> data)
    {
        float total = 0;

        for (int i = 0; i < data.Count; i++)
        {
            var pred = engine.Predict(data[i]).PrevHour;
            total += Math.Abs(data[i].Label - pred);
        }

        return total / data.Count;
    }
}