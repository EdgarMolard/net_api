using MonPremierProjet.Models;

namespace MonPremierProjet.Services;

public class TodoService : ITodoService
{
    private readonly List<Todo> todos = [];
    private int nextTodoId = 1;

    public IReadOnlyList<Todo> GetAll() => todos;

    public Todo? GetById(int id) => todos.FirstOrDefault(todo => todo.Id == id);

    public Todo Add(Todo todo)
    {
        todo.Id = nextTodoId++;
        todos.Add(todo);
        return todo;
    }

    public Todo? Update(int id, Todo updatedTodo)
    {
        var todo = GetById(id);
        if (todo is null)
        {
            return null;
        }

        todo.Title = updatedTodo.Title;
        todo.IsCompleted = updatedTodo.IsCompleted;
        return todo;
    }

    public bool Delete(int id)
    {
        var todo = GetById(id);
        return todo is not null && todos.Remove(todo);
    }
}
