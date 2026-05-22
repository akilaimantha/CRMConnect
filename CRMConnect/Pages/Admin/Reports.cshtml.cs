using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class ReportsModel : PageModel
{
    public List<ReportRow> SalesByStatus { get; set; } = new();
    public List<ReportRow> CustomersByStatus { get; set; } = new();
    public List<ReportRow> UsersByRole { get; set; } = new();
    public int TotalTasks { get; set; }
    public int PendingTasks { get; set; }
    public int TotalCommunications { get; set; }

    public void OnGet()
    {
        try
        {
            LoadGroup(SalesByStatus, "SELECT LeadStatus AS Label, COUNT(*) AS Cnt FROM SalesActivities GROUP BY LeadStatus");
            LoadGroup(CustomersByStatus, "SELECT Status AS Label, COUNT(*) AS Cnt FROM Customers GROUP BY Status");
            LoadGroup(UsersByRole, "SELECT Role AS Label, COUNT(*) AS Cnt FROM Users GROUP BY Role");
            TotalTasks = Count("SELECT COUNT(*) FROM Tasks");
            PendingTasks = Count("SELECT COUNT(*) FROM Tasks WHERE Status = 'Pending'");
            TotalCommunications = Count("SELECT COUNT(*) FROM CommunicationLog");
        }
        catch { }
    }

    private void LoadGroup(List<ReportRow> list, string sql)
    {
        var dt = DatabaseHelper.ExecuteQuery(sql);
        foreach (DataRow r in dt.Rows)
            list.Add(new ReportRow { Label = r["Label"]?.ToString() ?? "N/A", Count = Convert.ToInt32(r["Cnt"]) });
    }

    private int Count(string sql) => Convert.ToInt32(DatabaseHelper.ExecuteQuery(sql).Rows[0][0]);
}

public class ReportRow { public string Label { get; set; } = ""; public int Count { get; set; } }
