using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.Enum;

namespace VoltHome.Contracts.Requests.SolarStation;

public class SolarStationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid SolarRegionId { get; set; }
    public string SolarRegionName { get; set; }
    public InverterDto Inverter { get; set; }
    public List<PanelGroupDto> PanelGroups { get; set; }
}

public class InverterDto
{
    public double Power { get; set; }
    public double Efficiency { get; set; }
}

public class PanelGroupDto
{
    public Guid Id { get; set; }
    public int PanelCount { get; set; }
    public double PowerPerPanel { get; set; }
    public double TiltAngle { get; set; }
    public SolarAzimuth Azimuth { get; set; }
}
