namespace DevArt.Users.Core.Enums;

[Flags]
public enum ErrorCodes
{
    UserNotFoundException = 0b_1,
    UserBrokenAccessException = 0b_10,
    UserEmailExistingException = 0b_100
}