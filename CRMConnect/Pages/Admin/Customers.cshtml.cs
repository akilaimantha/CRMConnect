using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class CustomersModel : PageModel
{
    public List<CustomerVm> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet() => Load();

    public IActionResult OnPostDelete(int id)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Tasks WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            DatabaseHelper.ExecuteNonQuery("DELETE FROM SalesActivities WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            DatabaseHelper.ExecuteNonQuery("DELETE FROM CommunicationLog WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Customers WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            Message = "Invalid record removed."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        Load(); return Page();
    }

    private void Load()
    {
        Customers.Clear();
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT CustomerID, Name, Email, Phone, Company, Address, Status, CreatedDate FROM Customers ORDER BY Name");
        foreach (DataRow r in dt.Rows)
            Customers.Add(new CustomerVm {
                Id = Convert.ToInt32(r["CustomerID"]), Name = r["Name"]?.ToString() ?? "",
                Email = r["Email"]?.ToString() ?? "", Phone = r["Phone"]?.ToString() ?? "",
                Company = r["Company"]?.ToString() ?? "", Status = r["Status"]?.ToString() ?? "",
                Created = r["CreatedDate"] as DateTime?
            });
    }
}

public class CustomerVm
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Company { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime? Created { get; set; }
}
