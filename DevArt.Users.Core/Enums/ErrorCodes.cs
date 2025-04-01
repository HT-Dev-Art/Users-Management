namespace DevArt.Users.Core.Enums;

[Flags]
public enum ErrorCodes
{
    UserNotFoundException = 0b_1,
    FailedUpdateUserException = 0b_10,
}