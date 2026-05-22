using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Sales;

[SalesAuthorize]
public class TasksModel : PageModel
{
    public List<TaskVm> Tasks { get; set; } = new();
    public List<CustomerVm> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;
    public string RepName { get; set; } = "";

    public void OnGet() { RepName = SessionAuth.GetDisplayName(HttpContext); LoadCustomers(); LoadTasks(); }

    public IActionResult OnPost(string taskName, string deadline, int? customerId)
    {
        if (string.IsNullOrWhiteSpace(taskName)) { Message = "Task name required."; IsSuccess = false; LoadCustomers(); LoadTasks(); return Page(); }
        try
        {
            var due = string.IsNullOrEmpty(deadline) ? DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") : deadline;
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO Tasks (TaskID, TaskName, AssignedTo, CustomerID, Deadline, Status)
                  VALUES (TaskSeq.NEXTVAL, :tn, :at, :cid, TO_DATE(:dl, 'YYYY-MM-DD'), 'Pending')",
                new OracleParameter[] {
                    new("tn", taskName), new("at", SessionAuth.GetDisplayName(HttpContext)),
                    new("cid", customerId ?? (object)DBNull.Value), new("dl", due)
                });
            Message = "Task created."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        LoadCustomers(); LoadTasks(); return Page();
    }

    public IActionResult OnPostComplete(int id)
    {
        DatabaseHelper.ExecuteNonQuery("UPDATE Tasks SET Status = 'Completed' WHERE TaskID = :id", new[] { new OracleParameter("id", id) });
        LoadCustomers(); LoadTasks(); return Page();
    }

    public IActionResult OnPostUpdateStatus(int id, string status)
    {
        DatabaseHelper.ExecuteNonQuery("UPDATE Tasks SET Status = :s WHERE TaskID = :id",
            new[] { new OracleParameter("s", status), new OracleParameter("id", id) });
        LoadCustomers(); LoadTasks(); return Page();
    }

    private void LoadTasks()
    {
        Tasks.Clear();
        var dt = DatabaseHelper.ExecuteQuery(
            @"SELECT t.TaskID, t.TaskName, t.AssignedTo, t.Deadline, t.Status, c.Name AS CustomerName
              FROM Tasks t LEFT JOIN Customers c ON t.CustomerID = c.CustomerID ORDER BY t.Deadline ASC");
        foreach (DataRow r in dt.Rows)
            Tasks.Add(new TaskVm {
                Id = Convert.ToInt32(r["TaskID"]), Name = r["TaskName"]?.ToString() ?? "",
                Assigned = r["AssignedTo"]?.ToString() ?? "", Customer = r["CustomerName"]?.ToString() ?? "",
                Deadline = r["Deadline"] as DateTime?, Status = r["Status"]?.ToString() ?? ""
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

public class TaskVm
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Assigned { get; set; } = "";
    public string Customer { get; set; } = "";
    public DateTime? Deadline { get; set; }
    public string Status { get; set; } = "";
}
