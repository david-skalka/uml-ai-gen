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
public class AlarmTests
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory = new();

    public AlarmTests()
    {
        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
    }

    [SetUp]
    public void ClearDatabase()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        db.Alarms.ExecuteDelete();
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
        var response = await _client.GetAsync("/api/alarms");

        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<Alarm>>();
        Assert.That(items, Is.Empty);
    }

    [Test]
    public async Task Create_IsPersisted()
    {
        var time = new DateTime(2026, 5, 23, 8, 0, 0, DateTimeKind.Utc);
        var response = await _client.PostAsJsonAsync("/api/alarms", new CreateAlarmRequest
        {
            Title = "Wake up",
            Time = time
        });

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<Alarm>();
        Assert.That(created, Is.Not.Null);
        Assert.That(created!.Title, Is.EqualTo("Wake up"));
        Assert.That(created.Time, Is.EqualTo(time));
    }

    [Test]
    public async Task Update_ChangesFields()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/alarms", new CreateAlarmRequest
        {
            Title = "Original",
            Time = new DateTime(2026, 5, 23, 8, 0, 0, DateTimeKind.Utc)
        });
        var created = await createResponse.Content.ReadFromJsonAsync<Alarm>();

        var updatedTime = new DateTime(2026, 5, 23, 9, 0, 0, DateTimeKind.Utc);
        var response = await _client.PutAsJsonAsync($"/api/alarms/{created!.Id}", new UpdateAlarmRequest
        {
            Title = "Updated",
            Time = updatedTime
        });

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<Alarm>();
        Assert.That(updated!.Title, Is.EqualTo("Updated"));
        Assert.That(updated.Time, Is.EqualTo(updatedTime));
    }

    [Test]
    public async Task Delete_RemovesItem()
    {
        var createResponse = await _client.PostAsJsonAsync("/api/alarms", new CreateAlarmRequest
        {
            Title = "To delete",
            Time = new DateTime(2026, 5, 23, 8, 0, 0, DateTimeKind.Utc)
        });
        var created = await createResponse.Content.ReadFromJsonAsync<Alarm>();

        var response = await _client.DeleteAsync($"/api/alarms/{created!.Id}");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NoContent));
        Assert.That(await _client.GetAsync($"/api/alarms/{created.Id}"), Has.Property(nameof(HttpResponseMessage.StatusCode)).EqualTo(HttpStatusCode.NotFound));
    }
}
