namespace CRMConnect.Models;

public static class SessionAuth
{
    public const string RoleKey = "Role";
    public const string UserNameKey = "UserName";
    public const string DisplayNameKey = "DisplayName";
    public const string CustomerIdKey = "CustomerId";

    public static bool IsLoggedIn(HttpContext context) =>
        !string.IsNullOrEmpty(context.Session.GetString(RoleKey));

    public static bool IsAdmin(HttpContext context)
    {
        var role = context.Session.GetString(RoleKey);
        return role == "Admin" || role == "User" || role == "SalesRep";
    }

    public static bool IsCustomer(HttpContext context) =>
        context.Session.GetString(RoleKey) == "Customer";

    public static void SetAdminSession(HttpContext context, string username, string displayName, string role)
    {
        context.Session.SetString(RoleKey, role);
        context.Session.SetString(UserNameKey, username);
        context.Session.SetString(DisplayNameKey, displayName);
        context.Session.Remove(CustomerIdKey);
    }

    public static void SetCustomerSession(HttpContext context, int customerId, string name, string email)
    {
        context.Session.SetString(RoleKey, "Customer");
        context.Session.SetString(CustomerIdKey, customerId.ToString());
        context.Session.SetString(DisplayNameKey, name);
        context.Session.SetString(UserNameKey, email);
    }

    public static void Clear(HttpContext context)
    {
        context.Session.Clear();
    }

    public static int? GetCustomerId(HttpContext context)
    {
        var id = context.Session.GetString(CustomerIdKey);
        return int.TryParse(id, out var cid) ? cid : null;
    }
}
