using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class DashboardModel : PageModel
{
    public int TotalUsers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalSales { get; set; }
    public int ActiveUsers { get; set; }

    public void OnGet()
    {
        try
        {
            TotalUsers = Count("SELECT COUNT(*) FROM Users");
            TotalCustomers = Count("SELECT COUNT(*) FROM Customers");
            TotalSales = Count("SELECT COUNT(*) FROM SalesActivities");
            ActiveUsers = Count("SELECT COUNT(*) FROM Users WHERE Status = 'Active'");
        }
        catch { }
    }

    private int Count(string sql) => Convert.ToInt32(DatabaseHelper.ExecuteQuery(sql).Rows[0][0]);
}
