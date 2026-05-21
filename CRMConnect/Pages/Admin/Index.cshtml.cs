using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class DashboardModel : PageModel
{
    public int TotalCustomers { get; set; }
    public int PendingTasks { get; set; }
    public int TotalSales { get; set; }
    public List<DashboardTaskItem> RecentTasks { get; set; } = new();

    public void OnGet()
    {
        try
        {
            var dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM Customers");
            TotalCustomers = Convert.ToInt32(dt.Rows[0][0]);

            dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM Tasks WHERE Status = 'Pending'");
            PendingTasks = Convert.ToInt32(dt.Rows[0][0]);

            dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities");
            TotalSales = Convert.ToInt32(dt.Rows[0][0]);

            const string query = @"SELECT TaskDescription, AssignedTo, DueDate, Status 
                            FROM Tasks 
                            WHERE ROWNUM <= 5 
                            ORDER BY DueDate ASC";
            dt = DatabaseHelper.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                RecentTasks.Add(new DashboardTaskItem
                {
                    TaskDescription = row["TaskDescription"]?.ToString() ?? "",
                    AssignedTo = row["AssignedTo"]?.ToString() ?? "",
                    DueDate = row["DueDate"] as DateTime?,
                    Status = row["Status"]?.ToString() ?? ""
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Dashboard error: {ex.Message}");
        }
    }
}

public class DashboardTaskItem
{
    public string TaskDescription { get; set; } = "";
    public string AssignedTo { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "";
}
