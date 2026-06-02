using System.Net.Http.Json;
using TodoAppApi.Models;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.Integration;

public class AlarmTests : IntegrationApiTests
{
    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task GetAll()
    {
        var response = await Client.GetAsync("/api/alarms");

        response.EnsureSuccessStatusCode();
        var items = await response.Content.ReadFromJsonAsync<List<Alarm>>();
        Assert.That(items, Has.Count.EqualTo(DefaultSeeder.Alarms.Length));
    }


    [Test]
    public async Task Create()
    {
        var time = new DateTime(2026, 5, 23, 8, 0, 0, DateTimeKind.Utc);
        var response = await Client.PostAsJsonAsync("/api/alarms", new Alarm
        {
            Title = "Wake up",
            Time = time
        });

        response.EnsureSuccessStatusCode();
        var created = await response.Content.ReadFromJsonAsync<Alarm>();
        Assert.That(created!.Title, Is.EqualTo("Wake up"));
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Update()
    {
        var updatedTime = new DateTime(2026, 5, 23, 9, 0, 0, DateTimeKind.Utc);
        var response = await Client.PutAsJsonAsync("/api/alarms", new Alarm
        {
            Id = 1,
            Title = "Updated",
            Time = updatedTime
        });

        response.EnsureSuccessStatusCode();
        var updated = await response.Content.ReadFromJsonAsync<Alarm>();
        Assert.That(updated!.Title, Is.EqualTo("Updated"));
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Delete()
    {
        var response = await Client.DeleteAsync("/api/alarms/1");

        response.EnsureSuccessStatusCode();
    }
}