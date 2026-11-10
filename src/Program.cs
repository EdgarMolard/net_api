using MonPremierProjet.Endpoints;
using MonPremierProjet.Services;
using Microsoft.EntityFrameworkCore;
using MonPremierProjet.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;



var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Jwt:Key is missing.");

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();

Console.WriteLine($"\nApplication started at {DateTime.Now}\n");

app.Use(async (context, next) =>
{
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();
    await next();
    stopwatch.Stop();

    var statusCode = context.Response.StatusCode;
    var color = statusCode is >= 200 and < 300
        ? "\u001b[32m"
        : statusCode >= 400
            ? "\u001b[31m"
            : "\u001b[33m";

    Console.WriteLine(
        $"{color}Request {context.Request.Method} {context.Request.Path} => " +
        $"{statusCode} in {stopwatch.ElapsedMilliseconds}ms\u001b[0m");
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Todo API

TodoEndpoint.MapTodoEndpoints(app);

//Run the application
app.Run();
