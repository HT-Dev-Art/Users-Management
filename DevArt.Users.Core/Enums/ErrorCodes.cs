namespace DevArt.Users.Core.Enums;

[Flags]
public enum ErrorCodes
{
    UserNotFoundException = 0b_1,
    BadHttpClientException = 0b_10,
}
