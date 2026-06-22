using Microsoft.EntityFrameworkCore;
using TodoAppApi;
using TodoAppApi.Models;

namespace TodoAppTest.Integration.Seeders;

public class DefaultSeeder : ISeeder
{
    public static readonly TodoList[] TodoLists =
    [
        new() { Id = 1, Name = "Work", Description = "Work tasks" },
        new() { Id = 2, Name = "Work 2", Description = "Work tasks 2" },
        new() { Id = 3, Name = "Work 3", Description = "Work tasks 3" },
        new() { Id = 4, Name = "Work 4", Description = "Work tasks 4" },
        new() { Id = 5, Name = "Work 5", Description = "Work tasks 5" },
        new() { Id = 6, Name = "Work 6", Description = "Work tasks 6" },
        new() { Id = 7, Name = "Work 7", Description = "Work tasks 7" },
        new() { Id = 8, Name = "Work 8", Description = "Work tasks 8" },
        new() { Id = 9, Name = "Work 9", Description = "Work tasks 9" },
        new() { Id = 10, Name = "Work 10", Description = "Work tasks 10" }
    ];

    public static readonly Alarm[] Alarms =
    [
        new() { Id = 1, Title = "Wake up", Time = new DateTime(2026, 5, 23, 8, 0, 0, DateTimeKind.Utc) },
        new() { Id = 2, Title = "Wake up 2", Time = new DateTime(2026, 5, 23, 9, 0, 0, DateTimeKind.Utc) },
        new() { Id = 3, Title = "Wake up 3", Time = new DateTime(2026, 5, 23, 10, 0, 0, DateTimeKind.Utc) },
        new() { Id = 4, Title = "Wake up 4", Time = new DateTime(2026, 5, 23, 11, 0, 0, DateTimeKind.Utc) },
        new() { Id = 5, Title = "Wake up 5", Time = new DateTime(2026, 5, 23, 12, 0, 0, DateTimeKind.Utc) },
        new() { Id = 6, Title = "Wake up 6", Time = new DateTime(2026, 5, 23, 13, 0, 0, DateTimeKind.Utc) },
        new() { Id = 7, Title = "Wake up 7", Time = new DateTime(2026, 5, 23, 14, 0, 0, DateTimeKind.Utc) },
        new() { Id = 8, Title = "Wake up 8", Time = new DateTime(2026, 5, 23, 15, 0, 0, DateTimeKind.Utc) },
        new() { Id = 9, Title = "Wake up 9", Time = new DateTime(2026, 5, 23, 16, 0, 0, DateTimeKind.Utc) },
        new() { Id = 10, Title = "Wake up 10", Time = new DateTime(2026, 5, 23, 17, 0, 0, DateTimeKind.Utc) }
    ];

    public void Clear(ApplicationDbContext dbContext)
    {
        var tables = GetAll().Select(x => x.GetType()).Distinct().Reverse();
        foreach (var table in tables)
        {
            var entityType = dbContext.Model.FindEntityType(table);

            if (entityType != null)
                dbContext.Database.ExecuteSqlRaw("DELETE FROM " + entityType.GetTableName());
        }
    }

    public void Seed(ApplicationDbContext dbContext)
    {
        GetAll().ForEach(x =>
        {
            dbContext.Add(x);
            dbContext.SaveChanges();
        });
    }

    private static List<object> GetAll()
    {
        var all = new List<object>();
        all.AddRange(TodoLists);
        all.AddRange(Alarms);
        return all;
    }
}