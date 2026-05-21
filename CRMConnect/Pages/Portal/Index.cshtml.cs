using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Portal;

[CustomerAuthorize]
public class PortalHomeModel : PageModel
{
    public string CustomerName { get; set; } = "";
    public string PolicyType { get; set; } = "Life Protection";
    public int MyTasks { get; set; }
    public int MyCommunications { get; set; }
    public int MySales { get; set; }
    public List<PortalCommItem> RecentComms { get; set; } = new();

    public void OnGet()
    {
        var cid = SessionAuth.GetCustomerId(HttpContext);
        if (cid == null) return;

        try
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT Name, PolicyType FROM Customers WHERE CustomerID = :id",
                new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });
            if (dt.Rows.Count > 0)
            {
                CustomerName = dt.Rows[0]["Name"]?.ToString() ?? "Customer";
                PolicyType = dt.Rows[0]["PolicyType"]?.ToString() ?? "Life Protection";
            }

            dt = DatabaseHelper.ExecuteQuery(
                "SELECT COUNT(*) FROM Tasks WHERE CustomerID = :id AND Status = 'Pending'",
                new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });
            MyTasks = Convert.ToInt32(dt.Rows[0][0]);

            dt = DatabaseHelper.ExecuteQuery(
                "SELECT COUNT(*) FROM CommunicationLog WHERE CustomerID = :id",
                new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });
            MyCommunications = Convert.ToInt32(dt.Rows[0][0]);

            dt = DatabaseHelper.ExecuteQuery(
                "SELECT COUNT(*) FROM SalesActivities WHERE CustomerID = :id",
                new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });
            MySales = Convert.ToInt32(dt.Rows[0][0]);

            dt = DatabaseHelper.ExecuteQuery(
                @"SELECT CommType, Notes, CommunicationDate FROM (
                    SELECT CommType, Notes, CommunicationDate FROM CommunicationLog 
                    WHERE CustomerID = :id ORDER BY CommunicationDate DESC
                  ) WHERE ROWNUM <= 3",
                new[] { new Oracle.ManagedDataAccess.Client.OracleParameter("id", cid.Value) });

            foreach (DataRow row in dt.Rows)
            {
                var type = row["CommType"]?.ToString() ?? "Note";
                RecentComms.Add(new PortalCommItem
                {
                    CommType = type,
                    Notes = row["Notes"]?.ToString() ?? "",
                    Date = Convert.ToDateTime(row["CommunicationDate"]),
                    TypeClass = type.Contains("Phone") ? "phone" : type.Contains("Email") ? "email" : type.Contains("Meeting") ? "meeting" : "note"
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Portal error: {ex.Message}");
        }
    }
}

public class PortalCommItem
{
    public string CommType { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime Date { get; set; }
    public string TypeClass { get; set; } = "note";
}
