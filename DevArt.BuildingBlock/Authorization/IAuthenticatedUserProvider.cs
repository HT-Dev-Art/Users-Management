namespace DevArt.BuildingBlock.Authorization;

public interface IAuthenticatedUserProvider
{
    AuthenticatedUser User { get; }
}