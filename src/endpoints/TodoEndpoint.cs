using MonPremierProjet.Models;
using MonPremierProjet.Services;

namespace MonPremierProjet.Endpoints;

public static class TodoEndpoint
{
    public static void MapTodoEndpoints(this WebApplication app)
    {
        var maxTitleLength = app.Configuration.GetValue<int>(
            "TodoSettings:MaxTitleLength",
            100);

        app.MapGet("/todos", (ITodoService service) =>
        {
            return Results.Ok(service.GetAll());
        })
        .WithName("GetTodos");

        app.MapGet("/todos/{id}", (int id, ITodoService service) =>
        {
            var todo = service.GetById(id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        })
        .WithName("GetTodoById");

        app.MapPost("/todos", (Todo todo, ITodoService service) =>
        {
            var validationError = ValidateTitle(todo.Title, maxTitleLength);
            if (validationError is not null)
            {
                return Results.BadRequest(validationError);
            }

            var createdTodo = service.Add(todo);
            return Results.Created($"/todos/{createdTodo.Id}", createdTodo);
        })
        .WithName("CreateTodo");

        app.MapPut("/todos/{id}", (int id, Todo updatedTodo, ITodoService service) =>
        {
            var validationError = ValidateTitle(updatedTodo.Title, maxTitleLength);
            if (validationError is not null)
            {
                return Results.BadRequest(validationError);
            }

            var todo = service.Update(id, updatedTodo);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        })
        .WithName("UpdateTodo");

        app.MapDelete("/todos/{id}", (int id, ITodoService service) =>
        {
            return service.Delete(id) ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTodo");
    }

    private static string? ValidateTitle(string title, int maxTitleLength)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return "Title is required.";
        }

        return title.Length > maxTitleLength
            ? $"Title cannot exceed {maxTitleLength} characters."
            : null;
    }
}
