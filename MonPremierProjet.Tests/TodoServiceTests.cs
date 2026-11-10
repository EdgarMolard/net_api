using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MonPremierProjet.Data;
using MonPremierProjet.Models;
using MonPremierProjet.Services;

namespace MonPremierProjet.Tests;

public class TodoServiceTests
{
    [Fact]
    public async Task Add_AssignsAnId()
    {
        await using var context = CreateContext();
        var service = new TodoService(context);

        var todo = await service.AddAsync(new Todo
        {
            Title = "Apprendre les tests"
        });

        Assert.Equal(1, todo.Id);
    }

    [Fact]
    public async Task Delete_RemovesExistingTodo()
    {
        await using var context = CreateContext();
        var service = new TodoService(context);
        var todo = await service.AddAsync(new Todo { Title = "À supprimer" });

        var deleted = await service.DeleteAsync(todo.Id);

        Assert.True(deleted);
        Assert.Empty(await service.GetAllTodosAsync());
    }

    [Fact]
    public async Task GetById_ReturnsNullForUnknownTodo()
    {
        await using var context = CreateContext();
        var service = new TodoService(context);

        var todo = await service.GetByIdAsync(999);

        Assert.Null(todo);
    }

    private static AppDbContext CreateContext()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}