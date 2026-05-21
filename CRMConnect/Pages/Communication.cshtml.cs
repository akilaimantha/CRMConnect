using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages;

[AdminAuthorize]
public class CommunicationModel : PageModel
{
    public List<CommunicationRecord> Communications { get; set; } = new();
    public List<CustomerDropdown> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;
    public string SearchTerm { get; set; } = "";
    public string FilterType { get; set; } = "All";

    public void OnGet(string search = "", string type = "All")
    {
        SearchTerm = search;
        FilterType = type;
        LoadCustomers();
        LoadCommunications(search, type);
    }

    public IActionResult OnPost(int customerId, string commType, string notes)
    {
        if (customerId == 0)
        {
            Message = "Please select a customer!";
            IsSuccess = false;
            LoadCustomers();
            LoadCommunications();
            return Page();
        }

        if (string.IsNullOrEmpty(notes))
        {
            Message = "Please enter communication notes!";
            IsSuccess = false;
            LoadCustomers();
            LoadCommunications();
            return Page();
        }

        try
        {
            string query = @"INSERT INTO CommunicationLog (LogID, CustomerID, CommunicationDate, Notes, CommType) 
                            VALUES (LogSeq.NEXTVAL, :CustomerID, SYSDATE, :Notes, :CommType)";
            
            OracleParameter[] parameters = {
                new OracleParameter("CustomerID", customerId),
                new OracleParameter("Notes", notes),
                new OracleParameter("CommType", commType)
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            Message = "✅ Communication logged successfully!";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = $"❌ Error: {ex.Message}";
            IsSuccess = false;
        }
        
        LoadCustomers();
        LoadCommunications();
        return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        DatabaseHelper.ExecuteNonQuery($"DELETE FROM CommunicationLog WHERE LogID = {id}");
        LoadCustomers();
        LoadCommunications();
        return Page();
    }

    private void LoadCommunications(string search = "", string type = "All")
    {
        Communications.Clear();
        
        string query = @"SELECT c.LogID, c.CommunicationDate, c.Notes, c.CommType,
                                cust.Name as CustomerName
                        FROM CommunicationLog c
                        INNER JOIN Customers cust ON c.CustomerID = cust.CustomerID
                        WHERE 1=1";
        
        if (!string.IsNullOrEmpty(search))
        {
            query += $" AND UPPER(cust.Name) LIKE '%{search.ToUpper()}%'";
        }
        
        if (type != "All")
        {
            query += $" AND c.CommType = '{type.Replace("'", "''")}'";
        }
        
        query += " ORDER BY c.CommunicationDate DESC";
        
        DataTable dt = DatabaseHelper.ExecuteQuery(query);
        
        foreach (DataRow row in dt.Rows)
        {
            string commType = row["CommType"]?.ToString() ?? "Note";
            Communications.Add(new CommunicationRecord
            {
                LogID = Convert.ToInt32(row["LogID"]),
                CommunicationDate = Convert.ToDateTime(row["CommunicationDate"]),
                Notes = row["Notes"]?.ToString() ?? "",
                CommType = commType,
                CustomerName = row["CustomerName"]?.ToString() ?? "",
                CommTypeClass = GetCommTypeClass(commType),
                CommTypeIcon = GetCommTypeIcon(commType)
            });
        }
    }

    private void LoadCustomers()
    {
        Customers.Clear();
        DataTable dt = DatabaseHelper.ExecuteQuery("SELECT CustomerID, Name FROM Customers ORDER BY Name");
        foreach (DataRow row in dt.Rows)
        {
            Customers.Add(new CustomerDropdown
            {
                CustomerID = Convert.ToInt32(row["CustomerID"]),
                Name = row["Name"]?.ToString() ?? ""
            });
        }
    }

    private string GetCommTypeClass(string type)
    {
        if (type.Contains("Phone")) return "phone";
        if (type.Contains("Email")) return "email";
        if (type.Contains("Meeting")) return "meeting";
        return "note";
    }

    private string GetCommTypeIcon(string type)
    {
        if (type.Contains("Phone")) return "📞";
        if (type.Contains("Email")) return "✉️";
        if (type.Contains("Meeting")) return "🤝";
        return "📝";
    }
}

public class CommunicationRecord
{
    public int LogID { get; set; }
    public DateTime CommunicationDate { get; set; }
    public string Notes { get; set; } = "";
    public string CommType { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CommTypeClass { get; set; } = "";
    public string CommTypeIcon { get; set; } = "";
}