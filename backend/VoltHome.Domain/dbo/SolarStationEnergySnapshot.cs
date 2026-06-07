using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class SolarStationEnergySnapshot
{
    public Guid Id { get; set; }

    public Guid SolarStationId { get; set; }

    public DateTime HourStartUtc { get; set; }

    public double GeneratedKwh { get; set; }

    public double SolarCoefficient { get; set; }

}
