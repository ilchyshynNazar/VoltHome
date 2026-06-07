using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Domain.dbo;

public class SolarStation
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }
    public ApplicationUser Owner { get; set; }

    public string Name { get; set; }

    public Guid SolarRegionId { get; set; }
    public SolarRegion SolarRegion { get; set; }


    public ICollection<PanelGroup> PanelGroups { get; set; }
    public Inverter Inverter { get; set; }

    public DateTime CreatedAt { get; set; }
}
