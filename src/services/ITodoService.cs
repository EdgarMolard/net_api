using MonPremierProjet.Models;

namespace MonPremierProjet.Services;

public interface ITodoService
{
    IReadOnlyList<Todo> GetAll();
    Todo? GetById(int id);
    Todo Add(Todo todo);
    Todo? Update(int id, Todo updatedTodo);
    bool Delete(int id);
}
