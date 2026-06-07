using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Services.Neural.Model;

public class NeuralInputModel
{
    public float Hour { get; set; }

    public float Month { get; set; }

    public float PanelPowerKw { get; set; }

    public float InverterPowerKw { get; set; }

    public float HourCoefficient { get; set; }

    public float MonthCoefficient { get; set; }

    public float AzimuthFactor { get; set; }

    public float TiltFactor { get; set; }

    public float Label { get; set; }
}
