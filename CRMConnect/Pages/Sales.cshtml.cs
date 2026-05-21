using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages;

public class SalesModel : PageModel
{
    public List<SalesActivity> SalesActivities { get; set; } = new();
    public List<CustomerDropdown> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;
    
    // Statistics
    public int TotalLeads { get; set; }
    public int HotLeads { get; set; }
    public int ConversionRate { get; set; }
    public int EstimatedRevenue { get; set; }
    public int LeadCount { get; set; }
    public int QualifiedCount { get; set; }
    public int ProposalCount { get; set; }
    public int ClosedCount { get; set; }

    public void OnGet(string filter = "All", string sort = "DESC")
    {
        LoadCustomers();
        LoadSalesActivities(filter, sort);
        LoadStatistics();
    }

    public IActionResult OnPost(int customerId, string leadStatus, string notes)
    {
        if (customerId == 0)
        {
            Message = "Please select a customer!";
            IsSuccess = false;
            LoadCustomers();
            LoadSalesActivities();
            LoadStatistics();
            return Page();
        }

        try
        {
            string query = @"INSERT INTO SalesActivities (ActivityID, CustomerID, ActivityDate, LeadStatus, Notes) 
                            VALUES (ActivitySeq.NEXTVAL, :CustomerID, SYSDATE, :LeadStatus, :Notes)";
            
            OracleParameter[] parameters = {
                new OracleParameter("CustomerID", customerId),
                new OracleParameter("LeadStatus", leadStatus),
                new OracleParameter("Notes", notes ?? "")
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            Message = "✅ Sales activity logged successfully!";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = $"❌ Error: {ex.Message}";
            IsSuccess = false;
        }
        
        LoadCustomers();
        LoadSalesActivities();
        LoadStatistics();
        return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        DatabaseHelper.ExecuteNonQuery($"DELETE FROM SalesActivities WHERE ActivityID = {id}");
        LoadCustomers();
        LoadSalesActivities();
        LoadStatistics();
        return Page();
    }

    private void LoadSalesActivities(string filter = "All", string sort = "DESC")
    {
        SalesActivities.Clear();
        
        string query = @"SELECT a.ActivityID, a.ActivityDate, a.LeadStatus, a.Notes, 
                                c.Name as CustomerName
                        FROM SalesActivities a
                        INNER JOIN Customers c ON a.CustomerID = c.CustomerID";
        
        if (filter != "All")
        {
            query += $" WHERE a.LeadStatus = '{filter.Replace("'", "''")}'";
        }
        
        query += $" ORDER BY a.ActivityDate {sort}";
        
        DataTable dt = DatabaseHelper.ExecuteQuery(query);
        
        foreach (DataRow row in dt.Rows)
        {
            SalesActivities.Add(new SalesActivity
            {
                ActivityID = Convert.ToInt32(row["ActivityID"]),
                ActivityDate = Convert.ToDateTime(row["ActivityDate"]),
                LeadStatus = row["LeadStatus"]?.ToString() ?? "",
                Notes = row["Notes"]?.ToString() ?? "",
                CustomerName = row["CustomerName"]?.ToString() ?? ""
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

    private void LoadStatistics()
    {
        // Total leads
        DataTable dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities");
        TotalLeads = Convert.ToInt32(dt.Rows[0][0]);
        
        // Hot leads
        dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities WHERE LeadStatus = 'Hot Lead'");
        HotLeads = Convert.ToInt32(dt.Rows[0][0]);
        
        // Closed won
        dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities WHERE LeadStatus = 'Closed Won'");
        int closedWon = Convert.ToInt32(dt.Rows[0][0]);
        
        // Conversion rate
        ConversionRate = TotalLeads > 0 ? (closedWon * 100 / TotalLeads) : 0;
        
        // Estimated revenue
        EstimatedRevenue = closedWon * 1000;
        
        // Pipeline stages
        dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities WHERE LeadStatus IN ('Hot Lead', 'Warm Lead')");
        LeadCount = Convert.ToInt32(dt.Rows[0][0]);
        
        dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities WHERE LeadStatus = 'Negotiation'");
        QualifiedCount = Convert.ToInt32(dt.Rows[0][0]);
        
        dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities WHERE LeadStatus = 'Proposal Sent'");
        ProposalCount = Convert.ToInt32(dt.Rows[0][0]);
        
        dt = DatabaseHelper.ExecuteQuery("SELECT COUNT(*) FROM SalesActivities WHERE LeadStatus = 'Closed Won'");
        ClosedCount = Convert.ToInt32(dt.Rows[0][0]);
    }
}

public class SalesActivity
{
    public int ActivityID { get; set; }
    public DateTime ActivityDate { get; set; }
    public string LeadStatus { get; set; } = "";
    public string Notes { get; set; } = "";
    public string CustomerName { get; set; } = "";
}

public class CustomerDropdown
{
    public int CustomerID { get; set; }
    public string Name { get; set; } = "";
}