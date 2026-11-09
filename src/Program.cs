using MonPremierProjet.Endpoints;
using MonPremierProjet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSingleton<ITodoService, TodoService>();

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


// Todo API

TodoEndpoint.MapTodoEndpoints(app);

//Run the application

app.Run();
