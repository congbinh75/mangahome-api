using FluentValidation;
using MangaHome.Api.Common;
using MangaHome.Api.Models.Requests;
using MangaHome.Core.Values;

namespace MangaHome.Api.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterUserRequest>
{
    public RegisterUserValidator()
    {
        RuleFor(v => v.Username)
            .Length(Constants.UsernameMinLength, Constants.UsernameMaxLength)
                .WithMessage(Messages.VAL_USERNAME_LENGTH_NOT_VALID)
            .Matches(Regex.Username)
                .WithMessage(Messages.VAL_USERNAME_FORMAT_NOT_VALID)
            .NotEmpty();

        RuleFor(v => v.Email)
            .EmailAddress()
                .WithMessage(Messages.VAL_EMAIL_NOT_VALID)
            .NotEmpty();

        RuleFor(v => v.Password)
            .Length(Constants.PasswordMinLength, Constants.PasswordMaxLength)
                .WithMessage(Messages.VAL_PASSWORD_LENGTH_NOT_VALID)
            .Matches(Regex.Password)
                .WithMessage(Messages.VAL_PASSWORD_FORMAT_NOT_VALID)
            .NotEmpty();
    }
}