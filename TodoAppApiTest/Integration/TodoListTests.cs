using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoAppApi;
using TodoAppApi.Controllers;
using TodoAppApi.Models;
using TodoAppApiTest.Integration.Infrastructure;

namespace TodoAppApiTest.Integration;

[NonParallelizable]
public class TodoListTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory = new();

    public TodoListTests()
    {
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [SetUp]
    public void ClearDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.TodoLists.ExecuteDelete();
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GetAll_ReturnsEmpty()
    {
        var response = await _client.GetAsync("/api/todo-lists");

        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<TodoList>>();
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task Create_IsPersisted()
    {
        var response = await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest
        {
            Name = "Work",
            Description = "Work tasks"
        });

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TodoList>();
        Assert.That(created, Is.Not.Null);
        Assert.That(created!.Name, Is.EqualTo("Work"));
        Assert.That(created.IsArchived, Is.False);
    }

    [Test]
    public async Task Create_EmptyName_ReturnsValidationError()
    {
        var response = await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest
        {
            Name = string.Empty,
            Description = "Work tasks"
        });

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        var body = await response.Content.ReadAsStringAsync();
        Assert.That(body, Does.Contain("Name"));
    }

    [Test]
    public async Task Update_ChangesFields()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest
        {
            Name = "Original",
            Description = "Before"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoList>();

        var response = await _client.PutAsJsonAsync($"/api/todo-lists/{created!.Id}", new UpdateTodoListRequest
        {
            Name = "Updated",
            Description = "After"
        });

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<TodoList>();
        Assert.That(updated!.Name, Is.EqualTo("Updated"));
        Assert.That(updated.Description, Is.EqualTo("After"));
    }

    [Test]
    public async Task Delete_RemovesItem()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest
        {
            Name = "To delete"
        });
        var created = await createResponse.Content.ReadFromJsonAsync<TodoList>();

        var response = await _client.DeleteAsync($"/api/todo-lists/{created!.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(await _client.GetAsync($"/api/todo-lists/{created.Id}"), Has.Property(nameof(HttpResponseMessage.StatusCode)).EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task GroupByName_GroupsByName()
    {
        await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest { Name = "Work" });
        await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest { Name = "Work" });
        await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest { Name = "Home" });

        var response = await _client.PostAsJsonAsync("/api/todo-lists/group-by-name", new GroupByNameInput
        {
            IncludeArchived = false
        });

        response.EnsureSuccessStatusCode();
        var grouped = await response.Content.ReadFromJsonAsync<List<GroupByNameOutput>>();
        Assert.That(grouped, Has.Count.EqualTo(2));
        Assert.That(grouped!.Single(x => x.Name == "Work").Count, Is.EqualTo(2));
        Assert.That(grouped.Single(x => x.Name == "Home").Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GroupByName_ExcludesArchivedWhenRequested()
    {
        var workResponse = await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest { Name = "Work" });
        var work = await workResponse.Content.ReadFromJsonAsync<TodoList>();
        await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest { Name = "Work" });

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var archived = await db.TodoLists.SingleAsync(x => x.Id == work!.Id);
            archived.IsArchived = true;
            await db.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/todo-lists/group-by-name", new GroupByNameInput
        {
            IncludeArchived = false
        });

        response.EnsureSuccessStatusCode();
        var grouped = await response.Content.ReadFromJsonAsync<List<GroupByNameOutput>>();
        Assert.That(grouped, Has.Count.EqualTo(1));
        Assert.That(grouped![0].Name, Is.EqualTo("Work"));
        Assert.That(grouped[0].Count, Is.EqualTo(1));
    }

    [Test]
    public async Task GroupByName_IncludesArchivedWhenRequested()
    {
        var workResponse = await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest { Name = "Work" });
        var work = await workResponse.Content.ReadFromJsonAsync<TodoList>();
        await _client.PostAsJsonAsync("/api/todo-lists", new CreateTodoListRequest { Name = "Work" });

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var archived = await db.TodoLists.SingleAsync(x => x.Id == work!.Id);
            archived.IsArchived = true;
            await db.SaveChangesAsync();
        }

        var response = await _client.PostAsJsonAsync("/api/todo-lists/group-by-name", new GroupByNameInput
        {
            IncludeArchived = true
        });

        response.EnsureSuccessStatusCode();
        var grouped = await response.Content.ReadFromJsonAsync<List<GroupByNameOutput>>();
        Assert.That(grouped, Has.Count.EqualTo(1));
        Assert.That(grouped![0].Name, Is.EqualTo("Work"));
        Assert.That(grouped[0].Count, Is.EqualTo(2));
    }
}
