using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class SolarHourlyCoefficient
{
    public Guid Id { get; set; }

    public int Hour { get; set; }

    public double Coefficient { get; set; }
}
