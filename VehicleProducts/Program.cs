using Microsoft.EntityFrameworkCore;
using VehicleProducts.Db;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllersWithViews();

//// Adding Razor page services 
builder.Services.AddRazorPages(); // missing

/// <summary>
/// Adding Dependency Injection for database 
/// </summary>

builder.Services.AddDbContext<ProductDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDbContext")));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ProductDbContext>();

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

app.MapRazorPages(); 

app.Run();
