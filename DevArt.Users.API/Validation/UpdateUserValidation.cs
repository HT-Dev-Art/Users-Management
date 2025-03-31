using DevArt.Users.Application.Dto;
using FluentValidation;

namespace DevArt.Users.API.Validation;

public class UpdateUserValidation : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidation()
    {
        RuleFor(user => user.NickName).MaximumLength(100)
            .When(user => user.NickName is not null)
            .WithMessage("The maximum length of your nickname is 100 characters, your input has exceeded that limit.");

        RuleFor(user => user.NewPassword).Equal(user => user.PasswordConfirmation)
            .When(user => user.NewPassword is not null)
            .WithMessage("Your confirmation password have to match with your password.");
    }
}