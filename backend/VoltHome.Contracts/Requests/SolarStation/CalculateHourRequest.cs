using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Requests.SolarStation;

public class CalculateHourRequest
{
    public DateTime HourUtc { get; set; }
}
