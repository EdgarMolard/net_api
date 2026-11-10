using MonPremierProjet.Models;

namespace MonPremierProjet.Services;

public interface ITodoService
{
    Task<IReadOnlyList<Todo>> GetAllTodosAsync();
    Task<Todo?> GetByIdAsync(int id);
    Task<Todo> AddAsync(Todo todo);
    Task<Todo?> UpdateAsync(int id, Todo updatedTodo);
    Task<bool> DeleteAsync(int id);
}
