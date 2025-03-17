using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
var AuthenticationProviderKey="IdentityApiKey";
builder.Services.AddAuthentication().AddJwtBearer(AuthenticationProviderKey, options => {
    options.Authority = "https://localhost:5005";
    options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
});
builder.Services.AddOcelot();

var app = builder.Build();



app.MapControllers();

app.UseOcelot().Wait();
//app.MapGet("/", () => "Hello World!");

app.Run();
