using MonPremierProjet.Contracts;
using MonPremierProjet.Models;
using MonPremierProjet.Services;

namespace MonPremierProjet.Endpoints;

public static class TodoEndpoint
{
    public static void MapTodoEndpoints(this WebApplication app)
    {
        app.MapGet("/todos", async (ITodoService service) =>
        {
            var todos = await service.GetAllTodosAsync();
            return Results.Ok(todos.Select(ToResponse));
        })
        .WithName("GetTodos");

        app.MapGet("/todos/{id}", async (int id, ITodoService service) =>
        {
            var todo = await service.GetByIdAsync(id);
            return todo is not null
                ? Results.Ok(ToResponse(todo))
                : Results.NotFound();
        })
        .WithName("GetTodoById");

        app.MapPost("/todos", async (CreateTodoRequest request, ITodoService service) =>
        {
            var todo = new Todo
            {
                Title = request.Title,
                IsCompleted = request.IsCompleted
            };

            var createdTodo = await service.AddAsync(todo);
            var response = ToResponse(createdTodo);

            return Results.Created($"/todos/{createdTodo.Id}", response);
        })
        .WithName("CreateTodo").RequireAuthorization();

        app.MapPut("/todos/{id}", async (
            int id,
            UpdateTodoRequest request,
            ITodoService service) =>
        {
            var todo = new Todo
            {
                Title = request.Title,
                IsCompleted = request.IsCompleted
            };

            var updatedTodo = await service.UpdateAsync(id, todo);
            return updatedTodo is not null
                ? Results.Ok(ToResponse(updatedTodo))
                : Results.NotFound();
        })
        .WithName("UpdateTodo").RequireAuthorization();

        app.MapDelete("/todos/{id}", async (int id, ITodoService service) =>
        {
            return await service.DeleteAsync(id)
                ? Results.NoContent()
                : Results.NotFound();
        })
        .WithName("DeleteTodo").RequireAuthorization();
    }

    private static TodoResponse ToResponse(Todo todo)
    {
        return new TodoResponse
        {
            Id = todo.Id,
            Title = todo.Title,
            IsCompleted = todo.IsCompleted
        };
    }
}
