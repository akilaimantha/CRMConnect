using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Oracle.ManagedDataAccess.Client;
using CRMConnect.Models;

namespace CRMConnect.Pages.Portal;

[CustomerAuthorize]
public class SupportModel : PageModel
{
    public string Message { get; set; } = "";
    public bool IsSuccess { get; set; } = true;

    public void OnGet() { }

    public IActionResult OnPost(string subject, string body)
    {
        var cid = SessionAuth.GetCustomerId(HttpContext);
        if (cid == null) return RedirectToPage("/Index");

        try
        {
            var notes = $"[Support - {subject}] {body}";
            DatabaseHelper.ExecuteNonQuery(
                @"INSERT INTO CommunicationLog (LogID, CustomerID, CommunicationDate, Notes, CommType) 
                  VALUES (LogSeq.NEXTVAL, :cid, SYSDATE, :notes, 'Note')",
                new[]
                {
                    new OracleParameter("cid", cid.Value),
                    new OracleParameter("notes", notes)
                });
            Message = "Your support request has been submitted. An advisor will contact you soon.";
            IsSuccess = true;
        }
        catch (Exception ex)
        {
            Message = ex.Message;
            IsSuccess = false;
        }

        return Page();
    }
}
