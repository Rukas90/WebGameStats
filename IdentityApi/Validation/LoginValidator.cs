using FastEndpoints;
using FluentValidation;

using LoginRequest = IdentityApi.Requests.LoginRequest;

namespace IdentityApi.Validation;

internal class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(request => request.Email).NotEmpty().EmailAddress();
        RuleFor(request => request.Password).NotEmpty();
        RuleFor(request => request.CaptchaToken).NotEmpty();
    }
}