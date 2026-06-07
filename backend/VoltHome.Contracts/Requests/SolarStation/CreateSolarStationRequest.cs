using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using VoltHome.Domain.Enum;

namespace VoltHome.Contracts.Requests.SolarStation;

public class CreateSolarStationRequest
{
    public string Name { get; set; }

    public Guid SolarRegionId { get; set; }

    public CreateInverterDto Inverter { get; set; }

    public List<CreatePanelGroupDto> PanelGroups { get; set; }
}

public class CreateInverterDto
{
    public double Power { get; set; }
    public double Efficiency { get; set; }
}

public class CreatePanelGroupDto
{
    public int PanelCount { get; set; }
    public double PowerPerPanel { get; set; }
    public double TiltAngle { get; set; }
    public SolarAzimuth Azimuth { get; set; }
}

public class CreateSolarStationRequestValidator
    : AbstractValidator<CreateSolarStationRequest>
{
    public CreateSolarStationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.SolarRegionId)
            .NotEmpty();

        RuleFor(x => x.Inverter)
            .NotNull()
            .SetValidator(new CreateInverterDtoValidator());

        RuleFor(x => x.PanelGroups)
            .NotNull()
            .NotEmpty()
            .WithMessage("At least one panel group is required");

        RuleForEach(x => x.PanelGroups)
            .SetValidator(new CreatePanelGroupDtoValidator());
    }
}

public class CreateInverterDtoValidator
    : AbstractValidator<CreateInverterDto>
{
    public CreateInverterDtoValidator()
    {
        RuleFor(x => x.Power)
            .GreaterThan(0)
            .LessThanOrEqualTo(100_000); // 100 MW — запас

        RuleFor(x => x.Efficiency)
            .InclusiveBetween(0.5, 1.0);
    }
}

public class CreatePanelGroupDtoValidator
    : AbstractValidator<CreatePanelGroupDto>
{
    public CreatePanelGroupDtoValidator()
    {
        RuleFor(x => x.PanelCount)
            .GreaterThan(0)
            .LessThanOrEqualTo(10_000);

        RuleFor(x => x.PowerPerPanel)
            .GreaterThan(0)
            .LessThanOrEqualTo(1000); // 1000W

        RuleFor(x => x.TiltAngle)
            .InclusiveBetween(0, 90);

        RuleFor(x => x.Azimuth)
            .IsInEnum();
    }
}
