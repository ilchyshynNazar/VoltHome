using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Requests.Auth;

public class RegisterUserRequest
{
    public string UserName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PhoneNumber { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}
public abstract class RegisterUserRequestValidator<T>
    : AbstractValidator<T>
    where T : RegisterUserRequest
{
    protected RegisterUserRequestValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain digit");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Matches(@"^\+?[1-9]\d{7,14}$")
            .WithMessage("Invalid phone number format");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(50);
    }
}

public sealed class RegisterUserRequestValidator
    : RegisterUserRequestValidator<RegisterUserRequest>
{
}
