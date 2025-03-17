using Movies.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Hosting;
using Movies.API.Data;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});

builder.Services.AddDbContext<MoviesContext>(options =>
                options.UseInMemoryDatabase("Movie"));
builder.Services.AddAuthentication("Bearer").AddJwtBearer("Bearer",options=> {
    options.Authority = "https://localhost:5005";
    options.TokenValidationParameters = new TokenValidationParameters { ValidateAudience = false };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "movieClient", "movies_mvc_client"));
});

var app = builder.Build();

MoviesContextSeed.SeedDatabase(app);

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSwagger();

// Enable middleware to serve Swagger-UI (HTML, JS, CSS, etc.)
app.UseSwaggerUI(c =>
{
    // Specify the Swagger JSON endpoint
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty;  // Optional: make Swagger UI available at the root URL
});

app.Run();
