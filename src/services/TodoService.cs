using Microsoft.EntityFrameworkCore;
using MonPremierProjet.Models;
using MonPremierProjet.Data;

namespace MonPremierProjet.Services;

public class TodoService(AppDbContext context) : ITodoService
{
    private readonly DbSet<Todo> todos = context.Todos;

    public async Task<IReadOnlyList<Todo>> GetAllTodosAsync()
    {
        return await todos.ToListAsync();
    }

    public async Task<Todo?> GetByIdAsync(int id)
    {
        return await todos.FirstOrDefaultAsync(todo => todo.Id == id);
    }

    public async Task<Todo> AddAsync(Todo todo)
    {
        todos.Add(todo);
        await context.SaveChangesAsync();
        return todo;
    }

    public async Task<Todo?> UpdateAsync(int id, Todo updatedTodo)
    {
        var todo = await GetByIdAsync(id);
        if (todo is null)
        {
            return null;
        }

        todo.Title = updatedTodo.Title;
        todo.IsCompleted = updatedTodo.IsCompleted;
        await context.SaveChangesAsync();
        return todo;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var todo = await GetByIdAsync(id);
        if (todo is null)
        {
            return false;
        }

        todos.Remove(todo);
        await context.SaveChangesAsync();
        return true;
    }
}