using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Portal;

[CustomerAuthorize]
public class PoliciesModel : PageModel
{
    public string CustomerName { get; set; } = "";
    public string PolicyType { get; set; } = "Life Protection";
    public List<PolicySaleItem> SalesUpdates { get; set; } = new();

    public void OnGet()
    {
        var cid = SessionAuth.GetCustomerId(HttpContext);
        if (cid == null) return;

        var dt = DatabaseHelper.ExecuteQuery(
            "SELECT Name, PolicyType FROM Customers WHERE CustomerID = :id",
            new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });
        if (dt.Rows.Count > 0)
        {
            CustomerName = dt.Rows[0]["Name"]?.ToString() ?? "";
            PolicyType = dt.Rows[0]["PolicyType"]?.ToString() ?? "Life Protection";
        }

        dt = DatabaseHelper.ExecuteQuery(
            @"SELECT LeadStatus, Notes, ActivityDate FROM SalesActivities 
              WHERE CustomerID = :id ORDER BY ActivityDate DESC",
            new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });

        foreach (DataRow row in dt.Rows)
        {
            SalesUpdates.Add(new PolicySaleItem
            {
                LeadStatus = row["LeadStatus"]?.ToString() ?? "",
                Notes = row["Notes"]?.ToString() ?? "",
                ActivityDate = Convert.ToDateTime(row["ActivityDate"])
            });
        }
    }
}

public class PolicySaleItem
{
    public string LeadStatus { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime ActivityDate { get; set; }
}
