using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class SolarRegion
{
    public Guid Id { get; set; }

    public string Name { get; set; } 

    public double IrradiationKwhPerM2Year { get; set; }

    public double GenerationPerKwYear { get; set; }
}
