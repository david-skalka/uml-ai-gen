using Microsoft.EntityFrameworkCore;
using TodoAppApi.Migrations;

namespace TodoAppApi;

public class Program
{
    public static void Main(string[] args)
    {
        _ = typeof(ApplicationDbContextModelSnapshot);
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddProblemDetails();
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.CustomOperationIds(api =>
                $"{api.ActionDescriptor.RouteValues["controller"]}{api.ActionDescriptor.RouteValues["action"]}");
        });

        if (!builder.Environment.IsEnvironment("Integration"))
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("TodoAppApiDb")));

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseExceptionHandler();
        app.MapControllers();

        if (!app.Environment.IsEnvironment("Integration"))
        {
            using var scope = app.Services.CreateScope();
            scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
        }

        app.Run();
    }
}