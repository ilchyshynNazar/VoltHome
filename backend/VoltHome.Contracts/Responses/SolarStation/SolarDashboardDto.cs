using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Responses.SolarStation;

public class SolarDashboardDto
{
    public double CurrentHourKwh { get; set; }
    public double TodayKwh { get; set; }

    public double ForecastNextHourKwh { get; set; }
    public double ForecastTodayRemainingKwh { get; set; }

    public double HourGaugeMax { get; set; }
    public double DayGaugeMax { get; set; }
    public double ForecastGaugeMax { get; set; }
}
