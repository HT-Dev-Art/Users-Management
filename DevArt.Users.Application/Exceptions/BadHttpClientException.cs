using DevArt.Users.Core.Enums;

namespace DevArt.Users.Application.Exceptions;

public class BadHttpClientException(string message) : UserDefinedException(ErrorCodes.BadHttpClientException, message)
{
}
