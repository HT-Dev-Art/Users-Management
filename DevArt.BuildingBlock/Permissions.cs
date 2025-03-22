namespace DevArt.BuildingBlock;

public static class Permissions
{
    public const string ClaimType = "permissions";

    public const string ReadCurrentUser = "read:current_user";
    

    public const string WriteCurrentUser = "write:current_user";

    public static readonly IReadOnlyDictionary<string, PermissionsFlags> PermissionssDictionary =
        new Dictionary<string, PermissionsFlags>
        {
            { ReadCurrentUser, PermissionsFlags.ReadCurrentUser },
            { WriteCurrentUser, PermissionsFlags.WriteCurrentUser },
        };
}

[Flags]
public enum PermissionsFlags
{
    None = 0b_0,
    ReadCurrentUser = 0b_1,
    WriteCurrentUser = 0b_100,
}