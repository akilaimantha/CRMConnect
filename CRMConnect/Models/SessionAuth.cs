namespace CRMConnect.Models;

public static class SessionAuth
{
    public const string RoleKey = "Role";
    public const string UserIdKey = "UserId";
    public const string UserNameKey = "Username";
    public const string DisplayNameKey = "DisplayName";

    public static bool IsLoggedIn(HttpContext context) =>
        !string.IsNullOrEmpty(context.Session.GetString(RoleKey));

    public static bool IsAdmin(HttpContext context) =>
        context.Session.GetString(RoleKey) == "Admin";

    public static bool IsSales(HttpContext context) =>
        context.Session.GetString(RoleKey) == "Sales";

    public static void SetUserSession(HttpContext context, int userId, string username, string displayName, string role)
    {
        context.Session.SetString(UserIdKey, userId.ToString());
        context.Session.SetString(UserNameKey, username);
        context.Session.SetString(DisplayNameKey, displayName);
        context.Session.SetString(RoleKey, role);
    }

    public static void Clear(HttpContext context) => context.Session.Clear();

    public static int? GetUserId(HttpContext context)
    {
        var id = context.Session.GetString(UserIdKey);
        return int.TryParse(id, out var uid) ? uid : null;
    }

    public static string GetDisplayName(HttpContext context) =>
        context.Session.GetString(DisplayNameKey) ?? "User";
}
