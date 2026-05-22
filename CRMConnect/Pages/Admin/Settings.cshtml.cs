using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using CRMConnect.Models;

namespace CRMConnect.Pages.Admin;

[AdminAuthorize]
public class SettingsModel : PageModel
{
    public List<SettingVm> Settings { get; set; } = new();
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;
    public string ConnectionDisplay { get; set; } = "";

    public void OnGet()
    {
        ConnectionDisplay = Mask(DatabaseHelper.ConnectionString);
        Load();
    }

    public IActionResult OnPost(int[] ids, string[] values)
    {
        try
        {
            for (int i = 0; i < ids.Length; i++)
                DatabaseHelper.ExecuteNonQuery("UPDATE Settings SET SettingValue = :v WHERE SettingID = :id",
                    new[] { new OracleParameter("v", values.Length > i ? values[i] : ""), new OracleParameter("id", ids[i]) });
            Message = "Settings saved."; IsSuccess = true;
        }
        catch (Exception ex) { Message = ex.Message; IsSuccess = false; }
        ConnectionDisplay = Mask(DatabaseHelper.ConnectionString);
        Load(); return Page();
    }

    private void Load()
    {
        Settings.Clear();
        try
        {
            var dt = DatabaseHelper.ExecuteQuery("SELECT SettingID, SettingName, SettingValue FROM Settings ORDER BY SettingName");
            foreach (DataRow r in dt.Rows)
                Settings.Add(new SettingVm {
                    Id = Convert.ToInt32(r["SettingID"]),
                    Name = r["SettingName"]?.ToString() ?? "",
                    Value = r["SettingValue"]?.ToString() ?? ""
                });
        }
        catch { }
    }

    private static string Mask(string cs) =>
        cs.Contains("Password=") ? System.Text.RegularExpressions.Regex.Replace(cs, @"Password=[^;]+", "Password=****") : cs;
}

public class SettingVm { public int Id { get; set; } public string Name { get; set; } = ""; public string Value { get; set; } = ""; }
