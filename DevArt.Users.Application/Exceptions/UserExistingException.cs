using DevArt.Users.Core.Enums;

namespace DevArt.Users.Application.Exceptions;

public class UserExistingException(string message) : UserDefinedException(ErrorCodes.UserEmailExistingException, message)
{
    
}