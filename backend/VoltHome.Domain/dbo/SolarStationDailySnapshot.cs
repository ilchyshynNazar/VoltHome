using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class SolarStationDailySnapshot
{
    public Guid Id { get; set; }

    public Guid SolarStationId { get; set; }

    public DateTime Day { get; set; }

    public double TotalKwh { get; set; }
}
