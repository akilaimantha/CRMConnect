using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CRMConnect.Models;

namespace CRMConnect.Pages;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        SessionAuth.Clear(HttpContext);
        return RedirectToPage("/Index");
    }
}
