using DevArt.Users.Core.Enums;

namespace DevArt.Users.Application.Exceptions;

public class UserNotFoundException(string message) : UserDefinedException(ErrorCodes.UserNotFoundException, message)
{
    
}