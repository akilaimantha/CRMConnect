using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class UsersModel : PageModel
{
    public List<SystemUser> Users { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet() => LoadUsers();

    public IActionResult OnPost(string username, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Message = "Username and password are required.";
            IsSuccess = false;
            LoadUsers();
            return Page();
        }

        try
        {
            var query = @"INSERT INTO AppUsers (UserID, Username, Password, Role)
                         VALUES (UserSeq.NEXTVAL, :Username, :Password, :Role)";
            DatabaseHelper.ExecuteNonQuery(query, new[]
            {
                new OracleParameter("Username", username),
                new OracleParameter("Password", password),
                new OracleParameter("Role", role ?? "User")
            });
            Message = "User created successfully.";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = $"Error: {ex.Message}";
            IsSuccess = false;
        }

        LoadUsers();
        return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM AppUsers WHERE UserID = :id",
                new[] { new OracleParameter("id", id) });
            Message = "User deleted.";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            IsSuccess = false;
        }
        LoadUsers();
        return Page();
    }

    private void LoadUsers()
    {
        Users.Clear();
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT UserID, Username, Role FROM AppUsers ORDER BY Username");
        foreach (DataRow row in dt.Rows)
        {
            Users.Add(new SystemUser
            {
                UserID = Convert.ToInt32(row["UserID"]),
                Username = row["Username"]?.ToString() ?? "",
                Role = row["Role"]?.ToString() ?? ""
            });
        }
    }
}

public class SystemUser
{
    public int UserID { get; set; }
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
}
