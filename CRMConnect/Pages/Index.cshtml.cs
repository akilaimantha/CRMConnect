using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages;

public class IndexModel : PageModel
{
    public int TotalCustomers { get; set; }
    public int PendingTasks { get; set; }
    public int TotalSales { get; set; }
    public List<TaskItem> RecentTasks { get; set; } = new();

    public void OnGet()
    {
        try
        {
            // Get total customers
            var dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM Customers");
            TotalCustomers = Convert.ToInt32(dt.Rows[0][0]);

            // Get pending tasks
            dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM Tasks WHERE Status = 'Pending'");
            PendingTasks = Convert.ToInt32(dt.Rows[0][0]);

            // Get total sales activities
            dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities");
            TotalSales = Convert.ToInt32(dt.Rows[0][0]);

            // Get recent tasks (5 most urgent)
            string query = @"SELECT TaskDescription, AssignedTo, DueDate, Status 
                            FROM Tasks 
                            WHERE ROWNUM <= 5 
                            ORDER BY DueDate ASC";
            dt = DatabaseHelper.ExecuteQuery(query);
            
            foreach (DataRow row in dt.Rows)
            {
                RecentTasks.Add(new TaskItem
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
            // Handle database connection issues gracefully
            TotalCustomers = 0;
            PendingTasks = 0;
            TotalSales = 0;
            Console.WriteLine($"Database Error: {ex.Message}");
        }
    }
}

public class TaskItem
{
    public string TaskDescription { get; set; } = "";
    public string AssignedTo { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "";
}