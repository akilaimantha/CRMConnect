using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages;

public class TasksModel : PageModel
{
    public List<TaskItem> Tasks { get; set; } = new();
    public List<CustomerSimple> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet()
    {
        LoadCustomers();
        LoadTasks();
    }

    public IActionResult OnPost(string taskDescription, string assignedTo, string dueDate, int? customerId)
    {
        if (string.IsNullOrEmpty(taskDescription))
        {
            Message = "Task description is required!";
            IsSuccess = false;
            LoadCustomers();
            LoadTasks();
            return Page();
        }

        try
        {
            string query = @"INSERT INTO Tasks (TaskID, AssignedTo, CustomerID, TaskDescription, DueDate, Status) 
                            VALUES (TaskSeq.NEXTVAL, :AssignedTo, :CustomerID, :TaskDesc, TO_DATE(:DueDate, 'YYYY-MM-DD'), 'Pending')";
            
            OracleParameter[] parameters = {
                new OracleParameter("AssignedTo", string.IsNullOrEmpty(assignedTo) ? "Unassigned" : assignedTo),
                new OracleParameter("CustomerID", customerId ?? (object)DBNull.Value),
                new OracleParameter("TaskDesc", taskDescription),
                new OracleParameter("DueDate", string.IsNullOrEmpty(dueDate) ? DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") : dueDate)
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            Message = "✅ Task created successfully!";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = $"❌ Error: {ex.Message}";
            IsSuccess = false;
        }
        
        LoadCustomers();
        LoadTasks();
        return Page();
    }

    public IActionResult OnPostComplete(int id)
    {
        DatabaseHelper.ExecuteNonQuery($"UPDATE Tasks SET Status = 'Completed' WHERE TaskID = {id}");
        LoadCustomers();
        LoadTasks();
        return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        DatabaseHelper.ExecuteNonQuery($"DELETE FROM Tasks WHERE TaskID = {id}");
        LoadCustomers();
        LoadTasks();
        return Page();
    }

    private void LoadTasks()
    {
        Tasks.Clear();
        string query = @"SELECT t.TaskID, t.TaskDescription, t.AssignedTo, t.DueDate, t.Status, c.Name as CustomerName 
                        FROM Tasks t 
                        LEFT JOIN Customers c ON t.CustomerID = c.CustomerID 
                        ORDER BY t.DueDate ASC";
        DataTable dt = DatabaseHelper.ExecuteQuery(query);
        
        foreach (DataRow row in dt.Rows)
        {
            Tasks.Add(new TaskItem
            {
                TaskID = Convert.ToInt32(row["TaskID"]),
                TaskDescription = row["TaskDescription"]?.ToString() ?? "",
                AssignedTo = row["AssignedTo"]?.ToString() ?? "",
                DueDate = row["DueDate"] as DateTime?,
                Status = row["Status"]?.ToString() ?? "",
                CustomerName = row["CustomerName"]?.ToString() ?? ""
            });
        }
    }

    private void LoadCustomers()
    {
        Customers.Clear();
        DataTable dt = DatabaseHelper.ExecuteQuery("SELECT CustomerID, Name FROM Customers");
        foreach (DataRow row in dt.Rows)
        {
            Customers.Add(new CustomerSimple
            {
                CustomerID = Convert.ToInt32(row["CustomerID"]),
                Name = row["Name"]?.ToString() ?? ""
            });
        }
    }
}

public class TaskItem
{
    public int TaskID { get; set; }
    public string TaskDescription { get; set; } = "";
    public string AssignedTo { get; set; } = "";
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } = "";
    public string CustomerName { get; set; } = "";
}

public class CustomerSimple
{
    public int CustomerID { get; set; }
    public string Name { get; set; } = "";
}