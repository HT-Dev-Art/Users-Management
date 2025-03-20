using DevArt.Users.Application.Dto;
using FluentValidation;

namespace DevArt.Users.API.Validation;

public class UpdateUserValidation : AbstractValidator<UpdateUserDto>
{
    public UpdateUserValidation()
    {
        RuleFor(user => user.NickName).MaximumLength(100)
            .When(user => user.NickName is not null)
            .WithMessage("The max length of nickname is 100. Your nick name has been exceeded.");

        RuleFor(user => user.NewPassword).Equal(user => user.PasswordConfirmation)
            .WithMessage("Your confirmation password have to match with your password");
    }
}