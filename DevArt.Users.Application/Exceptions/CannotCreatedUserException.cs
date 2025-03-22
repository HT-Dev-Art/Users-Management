using DevArt.Users.Core.Enums;

namespace DevArt.Users.Application.Exceptions;

public class CannotCreatedUserException(string message) : UserDefinedException(ErrorCodes.CannotCreatedUserException, message)
{
    
}