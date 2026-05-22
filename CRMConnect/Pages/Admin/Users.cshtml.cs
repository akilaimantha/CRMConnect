using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class UsersModel : PageModel
{
    public List<UserVm> Users { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet() => Load();

    public IActionResult OnPost(string name, string email, string username, string password, string role, string status)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        { Message = "Username and password required."; IsSuccess = false; Load(); return Page(); }
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO Users (UserID, Name, Email, Username, Password, Role, Status)
                  VALUES (UserSeq.NEXTVAL, :n, :e, :u, :p, :r, :s)",
                new OracleParameter[] {
                    new("n", name ?? ""), new("e", email ?? ""), new("u", username),
                    new("p", password), new("r", role ?? "Sales"), new("s", status ?? "Active")
                });
            Message = "User created."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        Load(); return Page();
    }

    public IActionResult OnPostEdit(int id, string name, string email, string role, string status)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                "UPDATE Users SET Name=:n, Email=:e, Role=:r, Status=:s WHERE UserID=:id",
                new OracleParameter[] { new("n", name), new("e", email ?? ""), new("r", role), new("s", status), new("id", id) });
            Message = "User updated."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        Load(); return Page();
    }

    public IActionResult OnPostResetPassword(int id, string newPassword)
    {
        if (string.IsNullOrWhiteSpace(newPassword)) { Message = "Enter new password."; IsSuccess = false; Load(); return Page(); }
        DatabaseHelper.ExecuteNonQuery("UPDATE Users SET Password = :p WHERE UserID = :id",
            new[] { new OracleParameter("p", newPassword), new OracleParameter("id", id) });
        Message = "Password reset."; IsSuccess = true;
        Load(); return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        DatabaseHelper.ExecuteNonQuery("DELETE FROM Users WHERE UserID = :id", new[] { new OracleParameter("id", id) });
        Message = "User deleted."; IsSuccess = true;
        Load(); return Page();
    }

    private void Load()
    {
        Users.Clear();
        var dt = DatabaseHelper.ExecuteQuery("SELECT UserID, Name, Email, Username, Role, Status FROM Users ORDER BY Name");
        foreach (DataRow r in dt.Rows)
            Users.Add(new UserVm {
                Id = Convert.ToInt32(r["UserID"]), Name = r["Name"]?.ToString() ?? "",
                Email = r["Email"]?.ToString() ?? "", Username = r["Username"]?.ToString() ?? "",
                Role = r["Role"]?.ToString() ?? "", Status = r["Status"]?.ToString() ?? ""
            });
    }
}

public class UserVm
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Username { get; set; } = "";
    public string Role { get; set; } = "";
    public string Status { get; set; } = "";
}
