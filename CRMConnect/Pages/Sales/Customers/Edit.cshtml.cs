using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Sales;

[SalesAuthorize]
public class CustomerEditModel : PageModel
{
    public CustomerVm Customer { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public IActionResult OnGet(int id)
    {
        Load(id);
        return Customer.Id > 0 ? Page() : RedirectToPage("/Sales/Customers");
    }

    public IActionResult OnPost(int id, string name, string email, string phone, string company, string address, string status)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                @"UPDATE Customers SET Name=:n, Email=:e, Phone=:p, Company=:co, Address=:a, Status=:s WHERE CustomerID=:id",
                new OracleParameter[] {
                    new("n", name), new("e", email ?? ""), new("p", phone ?? ""),
                    new("co", company ?? ""), new("a", address ?? ""), new("s", status), new("id", id)
                });
            Message = "Customer updated."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        Load(id); return Page();
    }

    private void Load(int id)
    {
        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT CustomerID, Name, Email, Phone, Company, Address, Status FROM Customers WHERE CustomerID = :id",
            new[] { new OracleParameter("id", id) });
        if (dt.Rows.Count == 0) return;
        var r = dt.Rows[0];
        Customer = new CustomerVm {
            Id = Convert.ToInt32(r["CustomerID"]), Name = r["Name"]?.ToString() ?? "",
            Email = r["Email"]?.ToString() ?? "", Phone = r["Phone"]?.ToString() ?? "",
            Company = r["Company"]?.ToString() ?? "", Address = r["Address"]?.ToString() ?? "",
            Status = r["Status"]?.ToString() ?? "Active"
        };
    }
}
