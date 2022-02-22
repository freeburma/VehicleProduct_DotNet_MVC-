using Microsoft.EntityFrameworkCore;
using VehicleProducts.Db;
using Microsoft.AspNetCore.Identity;
using VehicleProducts.Services;




var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//// Adding Razor page services 
builder.Services.AddRazorPages(); // missing

/// <summary>
/// Adding Dependency Injection for database 
/// </summary>
var connectionString = builder.Configuration.GetConnectionString("ProductDbContext");

builder.Services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(connectionString));

/// <summary>
/// Error: An unhandled exception has occurred while executing the request. System.InvalidOperationException: Unable to resolve service for type 'Microsoft.AspNetCore.Identity.UI.Services.IEmailSender' while attempting to activate 'VehicleProducts.Areas.Identity.Pages.Account.RegisterModel'.
/// Ref: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/roles?view=aspnetcore-6.0
/// </summary>
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // *** DotNet Core 6 => Identity Role 
    .AddEntityFrameworkStores<ProductDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

//app.UseHttpsRedirection(); // Redirects HTTP requests to HTTPS.
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

/// <summary>
/// Adding Role Services
/// Reference: https://docs.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-6.0
/// </summary>
using (var scope = app.Services.CreateScope())
{
    Task.Run(async () =>
   {
       await RoleServices.CreateRoles(scope.ServiceProvider);
   });


}// end using 

app.MapRazorPages();

app.Run();


public partial class Program { }
