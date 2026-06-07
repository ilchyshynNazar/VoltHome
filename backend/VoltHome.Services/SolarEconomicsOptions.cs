namespace VoltHome.Services;

public class SolarEconomicsOptions
{
    public const string SectionName = "SolarEconomics";

    public double CostPerKwInstalledUah { get; set; } = 22_000;

    public double GreenTariffUahPerKwh { get; set; } = 7.54;

    public string Disclaimer { get; set; } =
        "Оціночні значення: фактична окупність залежить від монтажу, інвертора, витрат і динаміки тарифу.";
}
