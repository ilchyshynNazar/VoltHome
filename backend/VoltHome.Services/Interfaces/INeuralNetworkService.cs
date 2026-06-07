using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Services.Interfaces;

public interface INeuralNetworkService
{
    Task<double> PredictAsync(
        int hour,
        int month,
        double panelPowerKw,
        double inverterPowerKw,
        double hourCoefficient,
        double monthCoefficient,
        double azimuthFactor,
        double tiltFactor,
        CancellationToken ct);
}
