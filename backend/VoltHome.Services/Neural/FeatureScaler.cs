using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Services.Neural;

public class FeatureScaler
{
    public float[] Transform(
        int hour,
        int month,
        double panelPowerKw,
        double inverterPowerKw,
        double hourCoefficient,
        double monthCoefficient,
        double azimuthFactor,
        double tiltFactor)
    {
        return new float[]
        {
            NormalizeHour(hour),
            NormalizeMonth(month),

            NormalizePower(panelPowerKw),
            NormalizePower(inverterPowerKw),

            (float)hourCoefficient,
            (float)monthCoefficient,

            (float)azimuthFactor,
            (float)tiltFactor
        };
    }

    private static float NormalizeHour(int hour)
        => hour / 23f;  

    private static float NormalizeMonth(int month)
        => month / 12f; 

    private static float NormalizePower(double value)
        => (float)Math.Log10(value + 1); 
}
