using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api;
using TodoApp.Views.Dialogs;
using TodoApp.Views.TodoList;
using TodoAppApi;
using TodoAppTest.E2e.Uitls;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

[Category("E2e")]
[NonParallelizable]
public class TodoListE2ETests
{
    private E2ESession? _session;

    [SetUp]
    public void SetUp()
    {
        _session = new E2ESession();
        _session.Start();

        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is not string seeder)
            return;

        var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
        seederInstance.Seed(
            _session.ApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>());
    }

    [TearDown]
    public void TearDown()
    {
        if (_session != null)
        {
            var db = _session.ApiFactory.Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is string seeder)
            {
                var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
                seederInstance.Clear(db);
            }
            else
                new DefaultSeeder().Clear(db);
        }

        _session?.Dispose();
        _session = null;
    }

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Show() =>
        _session!.RunOnPageAsync<TodoListPageView>(host =>
        {
            var grid = host.Page.TodoListsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));

            return Task.CompletedTask;
        });

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Create() =>
        _session!.RunOnPageAsync<TodoListPageView>(host =>
        {
            const string name = "Shopping";
            const string description = "Groceries";

            var grid = host.Page.TodoListsGrid;

            host.Page.NewButton.PerformClick();

            var edit = host.Window.WaitForDialog<MyDialogWindow, DialogTodoListEditView>().View;

            edit.NameTextBox.TypeText(name);
            edit.DescriptionTextBox.TypeText(description);

            edit.SaveButton.PerformClick();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<TodoList>().ToArray();
                items.Should().HaveCount(DefaultSeeder.TodoLists.Length + 1);
                items.Should().ContainSingle(x => x.Name == name && x.Description == description);
            });

            return Task.CompletedTask;
        });
}
