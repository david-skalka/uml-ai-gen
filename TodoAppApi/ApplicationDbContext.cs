using Microsoft.EntityFrameworkCore;
using TodoAppApi.Models;

namespace TodoAppApi;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<TodoList> TodoLists { get; set; } = null!;

    public DbSet<Alarm> Alarms { get; set; } = null!;
}