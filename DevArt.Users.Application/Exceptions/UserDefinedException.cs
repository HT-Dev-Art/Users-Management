using DevArt.Users.Core.Enums;

namespace DevArt.Users.Application.Exceptions;

public class UserDefinedException(ErrorCodes errorCode, string message) : Exception(message)
{
    
}