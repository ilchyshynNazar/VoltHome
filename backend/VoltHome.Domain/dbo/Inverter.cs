using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class Inverter
{
    public Guid Id { get; set; }

    public Guid SolarStationId { get; set; }
    public SolarStation SolarStation { get; set; }

    public double Power { get; set; }
    public double Efficiency { get; set; }
}
