using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Sales;

[SalesAuthorize]
public class DashboardModel : PageModel
{
    public int TotalCustomers { get; set; }
    public int PendingTasks { get; set; }
    public List<ActivityRow> RecentSales { get; set; } = new();
    public List<CommRow> RecentComms { get; set; } = new();
    public List<TaskRow> RecentTasks { get; set; } = new();

    public void OnGet()
    {
        try
        {
            TotalCustomers = Count("SELECT COUNT(*) FROM Customers WHERE Status = 'Active'");
            PendingTasks = Count("SELECT COUNT(*) FROM Tasks WHERE Status = 'Pending'");

            var dt = DatabaseHelper.ExecuteQuery(
                @"SELECT * FROM (
                    SELECT c.Name, a.LeadStatus, a.ActivityDate FROM SalesActivities a
                    JOIN Customers c ON a.CustomerID = c.CustomerID ORDER BY a.ActivityDate DESC
                  ) WHERE ROWNUM <= 5");
            foreach (DataRow r in dt.Rows)
                RecentSales.Add(new ActivityRow { Customer = r["Name"]?.ToString() ?? "", Status = r["LeadStatus"]?.ToString() ?? "", Date = Convert.ToDateTime(r["ActivityDate"]) });

            dt = DatabaseHelper.ExecuteQuery(
                @"SELECT * FROM (
                    SELECT c.Name, l.CommType, l.CommunicationDate, l.Notes FROM CommunicationLog l
                    JOIN Customers c ON l.CustomerID = c.CustomerID ORDER BY l.CommunicationDate DESC
                  ) WHERE ROWNUM <= 5");
            foreach (DataRow r in dt.Rows)
                RecentComms.Add(new CommRow { Customer = r["Name"]?.ToString() ?? "", Type = r["CommType"]?.ToString() ?? "", Date = Convert.ToDateTime(r["CommunicationDate"]), Notes = r["Notes"]?.ToString() ?? "" });

            dt = DatabaseHelper.ExecuteQuery(
                @"SELECT * FROM (
                    SELECT TaskName, AssignedTo, Deadline, Status FROM Tasks ORDER BY Deadline ASC
                  ) WHERE ROWNUM <= 5");
            foreach (DataRow r in dt.Rows)
                RecentTasks.Add(new TaskRow { Name = r["TaskName"]?.ToString() ?? "", Assigned = r["AssignedTo"]?.ToString() ?? "", Due = r["Deadline"] as DateTime?, Status = r["Status"]?.ToString() ?? "" });
        }
        catch { }
    }

    private int Count(string sql)
    {
        var dt = DatabaseHelper.ExecuteQuery(sql);
        return Convert.ToInt32(dt.Rows[0][0]);
    }
}

public class ActivityRow { public string Customer { get; set; } = ""; public string Status { get; set; } = ""; public DateTime Date { get; set; } }
public class CommRow { public string Customer { get; set; } = ""; public string Type { get; set; } = ""; public DateTime Date { get; set; } public string Notes { get; set; } = ""; }
public class TaskRow { public string Name { get; set; } = ""; public string Assigned { get; set; } = ""; public DateTime? Due { get; set; } public string Status { get; set; } = ""; }
