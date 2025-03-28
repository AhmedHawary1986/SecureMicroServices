﻿using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Movies.Client.ApiServices;
using Movies.Client.Data;
using Movies.Client.HttpHandlers;
using Duende.IdentityModel.Client;
using System.Security.Claims;
using Duende.IdentityModel;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MoviesClientContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesClientContext") ?? throw new InvalidOperationException("Connection string 'MoviesClientContext' not found.")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IMovieApiService, MovieApiService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
              .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
              .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
              {
                  options.Authority = "https://localhost:5005";

                  options.ClientId = "movies_mvc_client";
                  options.ClientSecret = "secret";
                  options.ResponseType = "code id_token";

                  //options.Scope.Add("openid");
                  //options.Scope.Add("profile");
                  options.Scope.Add("address");
                  options.Scope.Add("email");
                  options.Scope.Add("roles");

                  //options.ClaimActions.DeleteClaim("sid");
                  //options.ClaimActions.DeleteClaim("idp");
                  //options.ClaimActions.DeleteClaim("s_hash");
                  //options.ClaimActions.DeleteClaim("auth_time");
                  //options.ClaimActions.MapUniqueJsonKey("role", "role");

                  options.Scope.Add("movieAPI");

                  options.ClaimActions.MapUniqueJsonKey("role", "role");
                  options.SaveTokens = true;
                  options.GetClaimsFromUserInfoEndpoint = true;
                  options.TokenValidationParameters = new TokenValidationParameters { NameClaimType = JwtClaimTypes.GivenName, RoleClaimType = JwtClaimTypes.Role };
                 
              });

builder.Services.AddTransient<AuthenticationDelegatingHandler>();
builder.Services.AddTransient<HypridAuthenticationDelegatingHandler>();
builder.Services.AddHttpClient("MoviesClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5010/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
    .AddHttpMessageHandler<HypridAuthenticationDelegatingHandler>();

builder.Services.AddHttpClient("IDPClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5005/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddSingleton(new ClientCredentialsTokenRequest
{
    Address = "https://localhost:5005/connect/token",

    ClientId = "movieClient",
    ClientSecret = "secret",
    Scope = "movieAPI"
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Account/AccessDenied"; // Redirect here if user lacks permission
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
