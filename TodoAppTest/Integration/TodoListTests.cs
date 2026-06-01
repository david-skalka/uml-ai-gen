using System.Net.Http.Json;
using TodoAppApi.Controllers;
using TodoAppApi.Models;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.Integration;

public class TodoListTests : IntegrationApiTests
{
    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task GetAll()
    {
        var response = await Client.GetAsync("/api/todo-lists");

        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<TodoList>>();
        Assert.That(items, Has.Count.EqualTo(DefaultSeeder.TodoLists.Length));
    }



    [Test]
    public async Task Create()
    {
        var response = await Client.PostAsJsonAsync("/api/todo-lists", new TodoList
        {
            Name = "Work",
            Description = "Work tasks"
        });

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<TodoList>();
        Assert.That(created!.Name, Is.EqualTo("Work"));
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Update()
    {
        var response = await Client.PutAsJsonAsync("/api/todo-lists", new TodoList
        {
            Id = 1,
            Name = "Updated",
            Description = "After"
        });

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<TodoList>();
        Assert.That(updated!.Name, Is.EqualTo("Updated"));
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Delete()
    {
        var response = await Client.DeleteAsync($"/api/todo-lists/1");

        response.EnsureSuccessStatusCode();
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task GroupByName()
    {
        var response = await Client.PostAsJsonAsync("/api/todo-lists/group-by-name", new GroupByNameInput
        {
            IncludeArchived = false
        });

        response.EnsureSuccessStatusCode();
        var grouped = await response.Content.ReadFromJsonAsync<List<GroupByNameOutput>>();
        Assert.That(grouped, Has.Count.EqualTo(10));
    }
}
