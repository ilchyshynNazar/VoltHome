using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class SolarMonthlyCoefficient
{
    public Guid Id { get; set; }

    public int Month { get; set; }

    public double Coefficient { get; set; }
}
