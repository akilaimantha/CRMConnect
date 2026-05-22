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
        if (SessionAuth.IsAdmin(HttpContext)) return RedirectToPage("/Admin/Index");
        if (SessionAuth.IsSales(HttpContext)) return RedirectToPage("/Sales/Index");
        return Page();
    }

    public IActionResult OnPost(string username, string password)
    {
        try
        {
            var dt = DatabaseHelper.ExecuteQuery(
                @"SELECT UserID, Username, Name, Role, Status FROM Users 
                  WHERE Username = :u AND Password = :p AND Status = 'Active'",
                new[] {
                    new OracleParameter("u", username),
                    new OracleParameter("p", password)
                });

            if (dt.Rows.Count == 0)
            {
                ErrorMessage = "Invalid username or password.";
                return Page();
            }

            var row = dt.Rows[0];
            var role = row["Role"]?.ToString() ?? "";
            SessionAuth.SetUserSession(HttpContext,
                Convert.ToInt32(row["UserID"]),
                row["Username"]?.ToString() ?? username,
                row["Name"]?.ToString() ?? username,
                role);

            return role == "Admin"
                ? RedirectToPage("/Admin/Index")
                : RedirectToPage("/Sales/Index");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Database error: {ex.Message}. Run Database/CRMConnect_Schema.sql";
            return Page();
        }
    }
}
