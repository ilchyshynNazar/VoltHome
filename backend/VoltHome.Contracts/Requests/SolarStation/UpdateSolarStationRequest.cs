using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Requests.SolarStation;

public class UpdateSolarStationRequest
{
    public string Name { get; set; }
    public Guid SolarRegionId { get; set; }
}

public class UpdateSolarStationRequestValidator
    : AbstractValidator<UpdateSolarStationRequest>
{
    public UpdateSolarStationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.SolarRegionId)
            .NotEmpty();
    }
}

