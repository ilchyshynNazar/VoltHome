using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Services.Neural.Model;

public class MlModelProvider
{
    private readonly MLContext _ml;
    private readonly string _modelDir;

    public PredictionEngine<NeuralInputModel, NeuralPredictionModel> SolarEngine { get; }
    public PredictionEngine<RnnInputModel, RnnOutputModel> RnnEngine { get; }

    public MlModelProvider()
    {
        _ml = new MLContext(seed: 1);

        _modelDir = Path.Combine(AppContext.BaseDirectory, "Models");
        Directory.CreateDirectory(_modelDir);

        SolarEngine = LoadSolarModel();
        RnnEngine = LoadRnnModel();
    }

    private PredictionEngine<NeuralInputModel, NeuralPredictionModel> LoadSolarModel()
    {
        var path = Path.Combine(_modelDir, "solar-forecast-model.zip");

        if (!File.Exists(path))
        {
            Console.WriteLine("⚠ Solar model not found → using fallback model");

            return CreateFallbackSolarModel(path);
        }

        using var stream = File.OpenRead(path);
        var model = _ml.Model.Load(stream, out _);

        return _ml.Model.CreatePredictionEngine<
            NeuralInputModel,
            NeuralPredictionModel>(model);
    }

    private PredictionEngine<RnnInputModel, RnnOutputModel> LoadRnnModel()
    {
        var path = Path.Combine(_modelDir, "rnn-model.zip");

        if (!File.Exists(path))
        {
            Console.WriteLine("⚠ RNN model not found → using fallback");

            return CreateFallbackRnnModel(path);
        }

        using var stream = File.OpenRead(path);
        var model = _ml.Model.Load(stream, out _);

        return _ml.Model.CreatePredictionEngine<RnnInputModel, RnnOutputModel>(model);
    }

    private PredictionEngine<NeuralInputModel, NeuralPredictionModel> CreateFallbackSolarModel(string path)
    {
        var dummyData = new[]
        {
            new NeuralInputModel
            {
                Hour = 0,
                Month = 1,
                PanelPowerKw = 1,
                InverterPowerKw = 1,
                HourCoefficient = 1,
                MonthCoefficient = 1,
                AzimuthFactor = 1,
                TiltFactor = 1,
                Label = 1
            }
        };

        var data = _ml.Data.LoadFromEnumerable(dummyData);

        var pipeline =
            _ml.Transforms.Concatenate("Features",
                nameof(NeuralInputModel.Hour),
                nameof(NeuralInputModel.Month),
                nameof(NeuralInputModel.PanelPowerKw),
                nameof(NeuralInputModel.InverterPowerKw),
                nameof(NeuralInputModel.HourCoefficient),
                nameof(NeuralInputModel.MonthCoefficient),
                nameof(NeuralInputModel.AzimuthFactor),
                nameof(NeuralInputModel.TiltFactor))
            .Append(_ml.Regression.Trainers.Sdca());

        var model = pipeline.Fit(data);

        _ml.Model.Save(model, data.Schema, path);

        return _ml.Model.CreatePredictionEngine<
            NeuralInputModel,
            NeuralPredictionModel>(model);
    }

    private PredictionEngine<RnnInputModel, RnnOutputModel> CreateFallbackRnnModel(string path)
    {
        var dummy = new[]
        {
            new RnnInputModel
            {
                Sequence = new float[] { 1, 1, 1 },
                Label = 1
            }
        };

        var data = _ml.Data.LoadFromEnumerable(dummy);

        var pipeline =
            _ml.Transforms.Concatenate("Features", nameof(RnnInputModel.Sequence))
            .Append(_ml.Regression.Trainers.FastTree());

        var model = pipeline.Fit(data);

        _ml.Model.Save(model, data.Schema, path);

        return _ml.Model.CreatePredictionEngine<
            RnnInputModel,
            RnnOutputModel>(model);
    }

    public void DebugRnnOnDataset(List<RnnInputModel> dataset)
    {
        Console.WriteLine("\n==============================");
        Console.WriteLine("RNN TIME SERIES DEBUG");
        Console.WriteLine("==============================\n");

        Console.WriteLine("IDX | ACTUAL | PREDICT | ERROR");

        float totalError = 0;

        for (int i = 0; i < dataset.Count; i++)
        {
            var input = dataset[i];

            var pred = RnnEngine.Predict(input).Score;
            var actual = input.Label;
            var error = Math.Abs(actual - pred);

            totalError += error;

            Console.WriteLine(
                $"{i,3} | {actual,6:F2} | {pred,7:F2} | {error,6:F2}");
        }

        Console.WriteLine("\nAVG ERROR: " + (totalError / dataset.Count));
    }

    public void DebugErrorGraph(List<RnnInputModel> dataset)
    {
        Console.WriteLine("\nERROR GRAPH:\n");

        for (int i = 0; i < dataset.Count; i++)
        {
            var pred = RnnEngine.Predict(dataset[i]).Score;
            var error = Math.Abs(dataset[i].Label - pred);

            int bars = (int)(error * 3);

            Console.WriteLine($"{i,3} " + new string('█', bars));
        }
    }

    public void DebugTimeSeries(List<RnnInputModel> dataset)
    {
        Console.WriteLine("\nTIME SERIES COMPARISON:\n");

        for (int i = 0; i < dataset.Count; i++)
        {
            var pred = RnnEngine.Predict(dataset[i]).Score;
            var actual = dataset[i].Label;

            Console.WriteLine(
                $"{i,3} A:{actual,5:F1} P:{pred,5:F1} " +
                new string('*', (int)Math.Abs(actual - pred)));
        }
    }
}