using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages;

public class NotificationApiModel : PageModel
{
    public void OnGet()
    {
        string query = "SELECT COUNT(*) FROM Tasks WHERE Deadline <= SYSDATE + 3 AND Status = 'Pending'";
        DataTable dt = DatabaseHelper.ExecuteQuery(query);
        int count = Convert.ToInt32(dt.Rows[0][0]);
        
        Response.ContentType = "application/json";
        Response.WriteAsync(count.ToString());
    }
}