using System.Collections.Generic;

namespace VoltHome.Contracts.Responses.SolarStation;

public class SolarPaybackInsightDto
{
    public double PeakDcKw { get; set; }
    public double PeakAcKw { get; set; }
    public double EstimatedAnnualKwh { get; set; }
    public double EstimatedSystemCostUah { get; set; }
    public double AssumedGreenTariffUahPerKwh { get; set; }
    public double EstimatedAnnualRevenueUah { get; set; }
    public double RoughPaybackYears { get; set; }
    public IReadOnlyList<MonthlyIrradiationPointDto> MonthlyProfile { get; set; } = [];
    public string Disclaimer { get; set; } = string.Empty;
}

public class MonthlyIrradiationPointDto
{
    public int Month { get; set; }
    /// <summary>Seasonal factor from DB (0–1 scale in seed data, July ≈ 1).</summary>
    public double RelativeCoefficient { get; set; }
    /// <summary>Same coefficient as % of July peak for chart labeling.</summary>
    public double PercentOfPeak { get; set; }
}
