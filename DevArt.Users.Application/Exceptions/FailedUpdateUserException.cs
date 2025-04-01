using DevArt.Users.Core.Enums;

namespace DevArt.Users.Application.Exceptions;

public class FailedUpdateUserException( string message) : UserDefinedException(ErrorCodes.FailedUpdateUserException, message)
{
    
}