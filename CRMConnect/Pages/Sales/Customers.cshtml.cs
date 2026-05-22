using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Sales;

[SalesAuthorize]
public class CustomersModel : PageModel
{
    public List<CustomerVm> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;
    public string Search { get; set; } = "";

    public void OnGet(string? search) { Search = search ?? ""; Load(); }

    public IActionResult OnPost(string name, string email, string phone, string company, string address, string status)
    {
        if (string.IsNullOrWhiteSpace(name)) { Message = "Name is required."; IsSuccess = false; Load(); return Page(); }
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO Customers (CustomerID, Name, Email, Phone, Company, Address, Status, CreatedDate)
                  VALUES (CustomerSeq.NEXTVAL, :n, :e, :p, :co, :a, :s, SYSDATE)",
                new OracleParameter[] {
                    new("n", name), new("e", email ?? ""), new("p", phone ?? ""),
                    new("co", company ?? ""), new("a", address ?? ""), new("s", status ?? "Active")
                });
            Message = "Customer added."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        Load(); return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Tasks WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            DatabaseHelper.ExecuteNonQuery("DELETE FROM SalesActivities WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            DatabaseHelper.ExecuteNonQuery("DELETE FROM CommunicationLog WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            DatabaseHelper.ExecuteNonQuery("DELETE FROM Customers WHERE CustomerID = :id", new[] { new OracleParameter("id", id) });
            Message = "Customer deleted."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        Load(); return Page();
    }

    private void Load()
    {
        Customers.Clear();
        var sql = "SELECT CustomerID, Name, Email, Phone, Company, Address, Status FROM Customers WHERE 1=1";
        var pars = new List<OracleParameter>();
        if (!string.IsNullOrWhiteSpace(Search))
        {
            sql += " AND (UPPER(Name) LIKE :q OR UPPER(Email) LIKE :q OR UPPER(Company) LIKE :q)";
            pars.Add(new OracleParameter("q", $"%{Search.ToUpper()}%"));
        }
        sql += " ORDER BY Name";
        var dt = DatabaseHelper.ExecuteQuery(sql, pars.Count > 0 ? pars.ToArray() : null);
        foreach (DataRow r in dt.Rows)
            Customers.Add(new CustomerVm {
                Id = Convert.ToInt32(r["CustomerID"]), Name = r["Name"]?.ToString() ?? "",
                Email = r["Email"]?.ToString() ?? "", Phone = r["Phone"]?.ToString() ?? "",
                Company = r["Company"]?.ToString() ?? "", Address = r["Address"]?.ToString() ?? "",
                Status = r["Status"]?.ToString() ?? ""
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
    public string Address { get; set; } = "";
    public string Status { get; set; } = "";
}
