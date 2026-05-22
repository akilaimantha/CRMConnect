using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Sales;

[SalesAuthorize]
public class CommunicationModel : PageModel
{
    public List<CommVm> Logs { get; set; } = new();
    public List<CustomerVm> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet() { LoadCustomers(); LoadLogs(); }

    public IActionResult OnPost(int customerId, string commType, string notes)
    {
        if (customerId == 0 || string.IsNullOrWhiteSpace(notes)) { Message = "Customer and notes required."; IsSuccess = false; LoadCustomers(); LoadLogs(); return Page(); }
        try
        {
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO CommunicationLog (LogID, CustomerID, CommunicationDate, CommType, Notes)
                  VALUES (LogSeq.NEXTVAL, :c, SYSDATE, :t, :n)",
                new[] { new OracleParameter("c", customerId), new OracleParameter("t", commType), new OracleParameter("n", notes) });
            Message = "Communication saved."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        LoadCustomers(); LoadLogs(); return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        DatabaseHelper.ExecuteNonQuery("DELETE FROM CommunicationLog WHERE LogID = :id", new[] { new OracleParameter("id", id) });
        LoadCustomers(); LoadLogs(); return Page();
    }

    private void LoadLogs()
    {
        Logs.Clear();
        var dt = DatabaseHelper.ExecuteQuery(
            @"SELECT l.LogID, l.CommType, l.Notes, l.CommunicationDate, c.Name AS CustomerName
              FROM CommunicationLog l JOIN Customers c ON l.CustomerID = c.CustomerID ORDER BY l.CommunicationDate DESC");
        foreach (DataRow r in dt.Rows)
            Logs.Add(new CommVm {
                Id = Convert.ToInt32(r["LogID"]), Customer = r["CustomerName"]?.ToString() ?? "",
                Type = r["CommType"]?.ToString() ?? "", Notes = r["Notes"]?.ToString() ?? "",
                Date = Convert.ToDateTime(r["CommunicationDate"])
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

public class CommVm
{
    public int Id { get; set; }
    public string Customer { get; set; } = "";
    public string Type { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime Date { get; set; }
}
