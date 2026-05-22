using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Sales;

[SalesAuthorize]
public class SalesActivitiesModel : PageModel
{
    public List<SaleVm> Activities { get; set; } = new();
    public List<CustomerVm> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet() { LoadCustomers(); LoadActivities(); }

    public IActionResult OnPost(int customerId, string leadStatus, string notes)
    {
        if (customerId == 0) { Message = "Select a customer."; IsSuccess = false; LoadCustomers(); LoadActivities(); return Page(); }
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO SalesActivities (ActivityID, CustomerID, ActivityDate, LeadStatus, Notes)
                  VALUES (ActivitySeq.NEXTVAL, :c, SYSDATE, :s, :n)",
                new[] { new OracleParameter("c", customerId), new OracleParameter("s", leadStatus), new OracleParameter("n", notes ?? "") });
            Message = "Activity logged."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        LoadCustomers(); LoadActivities(); return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        DatabaseHelper.ExecuteNonQuery("DELETE FROM SalesActivities WHERE ActivityID = :id", new[] { new OracleParameter("id", id) });
        LoadCustomers(); LoadActivities(); return Page();
    }

    public IActionResult OnPostUpdateStatus(int id, string leadStatus)
    {
        DatabaseHelper.ExecuteNonQuery("UPDATE SalesActivities SET LeadStatus = :s WHERE ActivityID = :id",
            new[] { new OracleParameter("s", leadStatus), new OracleParameter("id", id) });
        LoadCustomers(); LoadActivities(); return Page();
    }

    private void LoadActivities()
    {
        Activities.Clear();
        var dt = DatabaseHelper.ExecuteQuery(
            @"SELECT a.ActivityID, a.ActivityDate, a.LeadStatus, a.Notes, c.Name AS CustomerName
              FROM SalesActivities a JOIN Customers c ON a.CustomerID = c.CustomerID ORDER BY a.ActivityDate DESC");
        foreach (DataRow r in dt.Rows)
            Activities.Add(new SaleVm {
                Id = Convert.ToInt32(r["ActivityID"]), Customer = r["CustomerName"]?.ToString() ?? "",
                Status = r["LeadStatus"]?.ToString() ?? "", Notes = r["Notes"]?.ToString() ?? "",
                Date = Convert.ToDateTime(r["ActivityDate"])
            });
    }

    private void LoadCustomers()
    {
        Customers.Clear();
        var dt = DatabaseHelper.ExecuteQuery("SELECT CustomerID, Name FROM Customers ORDER BY Name");
        foreach (DataRow r in dt.Rows)
            Customers.Add(new CustomerVm { Id = Convert.ToInt32(r["CustomerID"]), Name = r["Name"]?.ToString() ?? "" });
    }
}

public class SaleVm
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public string Status { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime Date { get; set; }
}
