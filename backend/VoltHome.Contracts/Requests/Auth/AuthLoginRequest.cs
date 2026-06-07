using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoltHome.Contracts.Requests.Auth;

public class AuthLoginRequest
{
    public string UserName { get; set; }
    public string Password { get; set; }
}
public class LoginModelValidator : AbstractValidator<AuthLoginRequest>
{
    public LoginModelValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
