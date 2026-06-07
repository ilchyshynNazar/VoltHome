using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Services.Interfaces;
using VoltHome.Services.Neural.Model;

namespace VoltHome.Services.Neural;

public class MlpNetworkService : INeuralNetworkService
{
    private readonly FeatureScaler _scaler;
    private readonly MlModelProvider _ml;

    public MlpNetworkService(
        FeatureScaler scaler,
        MlModelProvider ml)
    {
        _scaler = scaler;
        _ml = ml;
    }

    public Task<double> PredictAsync(
        int hour,
        int month,
        double panelPowerKw,
        double inverterPowerKw,
        double hourCoefficient,
        double monthCoefficient,
        double azimuthFactor,
        double tiltFactor,
        CancellationToken ct)
    {
        var features = _scaler.Transform(
            hour,
            month,
            panelPowerKw,
            inverterPowerKw,
            hourCoefficient,
            monthCoefficient,
            azimuthFactor,
            tiltFactor);

        var input = new NeuralInputModel
        {
            Hour = features[0],
            Month = features[1],

            PanelPowerKw = features[2],
            InverterPowerKw = features[3],

            HourCoefficient = features[4],
            MonthCoefficient = features[5],

            AzimuthFactor = features[6],
            TiltFactor = features[7]
        };

        var prediction = _ml.SolarEngine.Predict(input);

        var result = Math.Max(0, (double)prediction.Score);

        return Task.FromResult(result);
    }
}