using Microsoft.EntityFrameworkCore;
using ProjectManagement.Models;
using Microsoft.AspNetCore.Identity;
using ProjectManagement.Data;
using ProjectManagement.Areas.Identity.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ProjectManagement.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register your custom middleware
builder.Services.AddScoped<ExceptionHandlingMiddleware>();

builder.Services.AddDbContext<ProjectManagementContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<IdentityContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("IdentityContextConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<NotificationsHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}


app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();;
app.UseAuthorization();
app.MapHub<NotificationsHub>("/NotificationsHub");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "notifications",
    pattern: "Notifications/{action=Client}/{user?}",
    defaults: new { controller = "Notifications", action = "Client" });

app.MapRazorPages();


app.Run();
