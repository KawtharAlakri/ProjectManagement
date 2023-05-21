using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using IdentityContext = ProjectManagement.Data.IdentityContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("IdentityDbContextConnection")
    ));
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ProjectManagement.Data.IdentityContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
