using DevArt.Users.Core.Enums;

namespace DevArt.Users.Application.Exceptions;

public class UserBrokenAccessException( string message) : UserDefinedException(ErrorCodes.UserBrokenAccessException, message)
{
    
}