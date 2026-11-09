using MonPremierProjet.Models;
using MonPremierProjet.Services;

namespace MonPremierProjet.Tests;

public class TodoServiceTests
{
    [Fact]
    public void Add_AssignsAnId()
    {
        var service = new TodoService();

        var todo = service.Add(new Todo
        {
            Title = "Apprendre les tests"
        });

        Assert.Equal(1, todo.Id);
    }

    [Fact]
    public void Delete_RemovesExistingTodo()
    {
        var service = new TodoService();
        var todo = service.Add(new Todo { Title = "À supprimer" });

        var deleted = service.Delete(todo.Id);

        Assert.True(deleted);
        Assert.Empty(service.GetAll());
    }

    [Fact]
    public void GetById_ReturnsNullForUnknownTodo()
    {
        var service = new TodoService();

        var todo = service.GetById(999);

        Assert.Null(todo);
    }
}