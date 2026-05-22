using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRMConnect.Models;

[AttributeUsage(AttributeTargets.Class)]
public class AdminAuthorizeAttribute : Attribute, IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        if (!SessionAuth.IsAdmin(context.HttpContext))
            context.Result = new RedirectToPageResult("/Index");
    }
    public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
}

[AttributeUsage(AttributeTargets.Class)]
public class SalesAuthorizeAttribute : Attribute, IPageFilter
{
    public void OnPageHandlerSelected(PageHandlerSelectedContext context) { }
    public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
    {
        if (!SessionAuth.IsSales(context.HttpContext))
            context.Result = new RedirectToPageResult("/Index");
    }
    public void OnPageHandlerExecuted(PageHandlerExecutedContext context) { }
}
