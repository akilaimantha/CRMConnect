using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Portal;

[CustomerAuthorize]
public class CommunicationsModel : PageModel
{
    public List<CustomerCommRecord> Communications { get; set; } = new();

    public void OnGet()
    {
        var cid = SessionAuth.GetCustomerId(HttpContext);
        if (cid == null) return;

        var dt = DatabaseHelper.ExecuteQuery(
            @"SELECT CommType, Notes, CommunicationDate FROM CommunicationLog 
              WHERE CustomerID = :id ORDER BY CommunicationDate DESC",
            new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });

        foreach (DataRow row in dt.Rows)
        {
            var type = row["CommType"]?.ToString() ?? "Note";
            Communications.Add(new CustomerCommRecord
            {
                CommType = type,
                Notes = row["Notes"]?.ToString() ?? "",
                CommunicationDate = Convert.ToDateTime(row["CommunicationDate"]),
                TypeClass = type.Contains("Phone") ? "phone" : type.Contains("Email") ? "email" : type.Contains("Meeting") ? "meeting" : "note"
            });
        }
    }
}

public class CustomerCommRecord
{
    public string CommType { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime CommunicationDate { get; set; }
    public string TypeClass { get; set; } = "note";
}
