using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using VoltHome.Domain.Enum;

namespace VoltHome.Domain.dbo;

public class PanelGroup
{
    public Guid Id { get; set; }

    public Guid SolarStationId { get; set; }

    [JsonIgnore]
    public SolarStation SolarStation { get; set; }

    public int PanelCount { get; set; }
    public double PowerPerPanel { get; set; }

    public double TiltAngle { get; set; }
    public SolarAzimuth Azimuth { get; set; }   
}
