using CRMConnect.Models;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

app.MapGet("/api/notifications", async context =>
{
    try
    {
        // Query database for tasks due in next 3 days
        string query = "SELECT COUNT(*) FROM Tasks WHERE DueDate <= SYSDATE + 3 AND Status = 'Pending'";
        DataTable dt = DatabaseHelper.ExecuteQuery(query);
        int count = Convert.ToInt32(dt.Rows[0][0]);
        
        // Return the count as plain text
        await context.Response.WriteAsync(count.ToString());
    }
    catch (Exception ex)
    {
        // If database error, return 0
        Console.WriteLine($"Notification error: {ex.Message}");
        await context.Response.WriteAsync("0");
    }
});

app.Run();
