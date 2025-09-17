using Core.Validation;
using FastEndpoints;
using FluentValidation;
using IdentityApi.Requests;

namespace IdentityApi.Validation;

internal class RegisterValidator : Validator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Username).NotEmpty().MinimumLength(6);
        RuleFor(request => request.Password).SetValidator(new PasswordValidator());
        RuleFor(request => request.RepeatPassword).Equal(request => request.Password);
        RuleFor(request => request.CaptchaToken).NotEmpty();
    }
}