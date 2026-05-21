using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class SettingsModel : PageModel
{
    public List<SettingItem> Settings { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;
    public string ConnectionDisplay { get; set; } = "";

    public void OnGet()
    {
        ConnectionDisplay = MaskConnection(DatabaseHelper.ConnectionString);
        LoadSettings();
    }

    public IActionResult OnPost(int[] ids, string[] names, string[] values)
    {
        try
        {
            for (int i = 0; i < ids.Length; i++)
            {
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Settings SET SettingValue = :val WHERE SettingID = :id",
                    new[]
                    {
                        new OracleParameter("val", values.Length > i ? values[i] : ""),
                        new OracleParameter("id", ids[i])
                    });
            }
            Message = "Settings saved successfully.";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            IsSuccess = false;
        }

        ConnectionDisplay = MaskConnection(DatabaseHelper.ConnectionString);
        LoadSettings();
        return Page();
    }

    private void LoadSettings()
    {
        Settings.Clear();
        try
        {
            var dt = DatabaseHelper.ExecuteQuery(
                "SELECT SettingID, SettingName, SettingValue FROM Settings ORDER BY SettingName");
            foreach (DataRow row in dt.Rows)
            {
                Settings.Add(new SettingItem
                {
                    SettingID = Convert.ToInt32(row["SettingID"]),
                    SettingName = row["SettingName"]?.ToString() ?? "",
                    SettingValue = row["SettingValue"]?.ToString() ?? ""
                });
            }
        }
        catch
        {
            Settings.Add(new SettingItem
            {
                SettingID = 0,
                SettingName = "CompanyName",
                SettingValue = "Sri Lanka Insurance Corporation"
            });
        }
    }

    private static string MaskConnection(string cs)
    {
        if (cs.Contains("Password="))
            return System.Text.RegularExpressions.Regex.Replace(cs, @"Password=[^;]+", "Password=****");
        return cs;
    }
}

public class SettingItem
{
    public int SettingID { get; set; }
    public string SettingName { get; set; } = "";
    public string SettingValue { get; set; } = "";
}
