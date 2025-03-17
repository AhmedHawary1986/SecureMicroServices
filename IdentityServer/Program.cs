using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using IdentityServer;
using IdentityServerHost;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(options =>
{
    options.RootDirectory = "/Pages";
    //options.Conventions.AuthorizeFolder("/MyPages/Admin");
});
builder.Services.AddIdentityServer()
    .AddInMemoryClients(Config.Clients)
    .AddInMemoryIdentityResources(Config.IdentityResources)
    .AddInMemoryApiResources(Config.ApiResources)
    .AddInMemoryApiScopes(Config.ApiScopes)
    .AddTestUsers(TestUsers.Users)
    .AddDeveloperSigningCredential();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect here if user lacks permission
});
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseIdentityServer();
app.UseAuthorization();



// Set Identity Server home page as the default route
//app.MapGet("/", (context) =>
//{
//    context.Response.Redirect("/home");
//    return Task.CompletedTask;
//});

app.MapControllers();
app.MapRazorPages();

app.Run();
