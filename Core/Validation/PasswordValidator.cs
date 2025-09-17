using FluentValidation;

namespace Core.Validation;

public class PasswordValidator : AbstractValidator<string>
{
    public PasswordValidator()
    {
        RuleFor(p => p)
            .NotEmpty()
            .MinimumLength(12)
            .Matches(@"[A-Z]").WithMessage("Must contain uppercase.")
            .Matches(@"[a-z]").WithMessage("Must contain lowercase.")
            .Matches(@"\d").WithMessage("Must contain number.")
            .Matches(@"[\W_]").WithMessage("Must contain special character.");
    }
}