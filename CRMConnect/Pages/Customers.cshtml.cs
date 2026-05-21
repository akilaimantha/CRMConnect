using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages;

[AdminAuthorize]
public class CustomersModel : PageModel
{
    public List<Customer> Customers { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet()
    {
        LoadCustomers();
    }

    public IActionResult OnPost(string name, string email, string phone, string address, string policyType)
    {
        if (string.IsNullOrEmpty(name))
        {
            Message = "Customer name is required!";
            IsSuccess = false;
            LoadCustomers();
            return Page();
        }

        try
        {
            string query = @"INSERT INTO Customers (CustomerID, Name, Email, Phone, Address, PolicyType, LoginPassword, CreatedDate) 
                            VALUES (CustomerSeq.NEXTVAL, :Name, :Email, :Phone, :Address, :PolicyType, 'customer123', SYSDATE)";
            
            OracleParameter[] parameters = {
                new OracleParameter("Name", name),
                new OracleParameter("Email", email ?? ""),
                new OracleParameter("Phone", phone ?? ""),
                new OracleParameter("Address", address ?? ""),
                new OracleParameter("PolicyType", policyType ?? "Life Protection")
            };
            
            DatabaseHelper.ExecuteNonQuery(query, parameters);
            Message = "✅ Customer added successfully!";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = $"❌ Error: {ex.Message}";
            IsSuccess = false;
        }
        
        LoadCustomers();
        return Page();
    }

    public IActionResult OnPostDelete(int id)
    {
        try
        {
            DatabaseHelper.ExecuteNonQuery($"DELETE FROM Tasks WHERE CustomerID = {id}");
            DatabaseHelper.ExecuteNonQuery($"DELETE FROM SalesActivities WHERE CustomerID = {id}");
            DatabaseHelper.ExecuteNonQuery($"DELETE FROM CommunicationLog WHERE CustomerID = {id}");
            DatabaseHelper.ExecuteNonQuery($"DELETE FROM Customers WHERE CustomerID = {id}");
            Message = "✅ Customer deleted successfully!";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = $"❌ Error: {ex.Message}";
            IsSuccess = false;
        }
        
        LoadCustomers();
        return Page();
    }

    private void LoadCustomers()
    {
        Customers.Clear();
        DataTable dt = DatabaseHelper.ExecuteQuery("SELECT CustomerID, Name, Email, Phone FROM Customers ORDER BY Name");
        
        foreach (DataRow row in dt.Rows)
        {
            Customers.Add(new Customer
            {
                CustomerID = Convert.ToInt32(row["CustomerID"]),
                Name = row["Name"]?.ToString() ?? "",
                Email = row["Email"]?.ToString() ?? "",
                Phone = row["Phone"]?.ToString() ?? ""
            });
        }
    }
}

public class Customer
{
    public int CustomerID { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
}