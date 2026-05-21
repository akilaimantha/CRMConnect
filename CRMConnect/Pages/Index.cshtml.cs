using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using CRMConnect.Models;

namespace CRMConnect.Pages;

public class IndexModel : PageModel
{
    public string ErrorMessage { get; set; } = "";

    public IActionResult OnGet()
    {
        if (SessionAuth.IsAdmin(HttpContext))
            return RedirectToPage("/Admin/Index");
        if (SessionAuth.IsCustomer(HttpContext))
            return RedirectToPage("/Portal/Index");
        return Page();
    }

    public IActionResult OnPostAdminLogin(string username, string password)
    {
        try
        {
            var query = @"SELECT UserID, Username, Role FROM AppUsers 
                         WHERE Username = :Username AND Password = :Password";
            var dt = DatabaseHelper.ExecuteQuery(query, new[]
            {
                new OracleParameter("Username", username),
                new OracleParameter("Password", password)
            });

            if (dt.Rows.Count == 0)
            {
                ErrorMessage = "Invalid admin credentials. Try admin / admin123";
                return Page();
            }

            var row = dt.Rows[0];
            var role = row["Role"]?.ToString() ?? "Admin";
            var uname = row["Username"]?.ToString() ?? username;
            SessionAuth.SetAdminSession(HttpContext, uname, uname, role);
            return RedirectToPage("/Admin/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Database connection error: {ex.Message}. Run Database/CRMConnect_Schema.sql first.";
            return Page();
        }
    }

    public IActionResult OnPostCustomerLogin(string email, string password)
    {
        try
        {
            var query = @"SELECT CustomerID, Name, Email FROM Customers 
                         WHERE Email = :Email AND LoginPassword = :Password";
            var dt = DatabaseHelper.ExecuteQuery(query, new[]
            {
                new OracleParameter("Email", email),
                new OracleParameter("Password", password)
            });

            if (dt.Rows.Count == 0)
            {
                ErrorMessage = "Invalid customer credentials. Try contact@abc.com / customer123";
                return Page();
            }

            var row = dt.Rows[0];
            SessionAuth.SetCustomerSession(
                HttpContext,
                Convert.ToInt32(row["CustomerID"]),
                row["Name"]?.ToString() ?? "Customer",
                row["Email"]?.ToString() ?? email);
            return RedirectToPage("/Portal/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Database connection error: {ex.Message}. Run Database/CRMConnect_Schema.sql first.";
            return Page();
        }
    }
}
