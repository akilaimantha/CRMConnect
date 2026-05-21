using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Portal;

[CustomerAuthorize]
public class ProfileModel : PageModel
{
    public CustomerProfile Customer { get; set; } = new();

    public void OnGet()
    {
        var cid = SessionAuth.GetCustomerId(HttpContext);
        if (cid == null) return;

        try
        {
            var dt = DatabaseHelper.ExecuteQuery(
                @"SELECT CustomerID, Name, Email, Phone, Address, PolicyType 
                  FROM Customers WHERE CustomerID = :id",
                new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });

            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                Customer = new CustomerProfile
                {
                    CustomerID = Convert.ToInt32(row["CustomerID"]),
                    Name = row["Name"]?.ToString() ?? "",
                    Email = row["Email"]?.ToString() ?? "",
                    Phone = row["Phone"]?.ToString() ?? "",
                    Address = row["Address"]?.ToString() ?? "",
                    PolicyType = row["PolicyType"]?.ToString() ?? "Life Protection"
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

public class CustomerProfile
{
    public int CustomerID { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string PolicyType { get; set; } = "";
}
