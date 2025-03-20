namespace DevArt.BuildingBlock;

public static class Scopes
{
    public const string ClaimType = "scope";

    public const string ReadCurrentUser = "read:current_user";
    

    public const string WriteCurrentUser = "write:current_user";

    public static readonly IReadOnlyDictionary<string, ScopesFlags> ScopesDictionary =
        new Dictionary<string, ScopesFlags>
        {
            { ReadCurrentUser, ScopesFlags.ReadCurrentUser },
            { WriteCurrentUser, ScopesFlags.WriteCurrentUser },
        };
}

[Flags]
public enum ScopesFlags
{
    None = 0b_0,
    ReadCurrentUser = 0b_1,
    WriteCurrentUser = 0b_100,

    User = ReadCurrentUser | WriteCurrentUser,
}