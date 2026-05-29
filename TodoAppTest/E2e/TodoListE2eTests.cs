using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
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
        _session!.RunUiAsync(async () =>
        {
            var window = await _session.OpenMainWindowAsync();
            var page = window.GetVisualDescendants().OfType<TodoListPageView>().Single();
            var grid = page.TodoListsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));
        });

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Create() =>
        _session!.RunUiAsync(async () =>
        {
            const string name = "Shopping";
            const string description = "Groceries";

            var window = await _session.OpenMainWindowAsync();
            var page = window.GetVisualDescendants().OfType<TodoListPageView>().Single();
            var grid = page.TodoListsGrid;

            page.NewButton.Focus();
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            window.Should().EventuallySatisfy(_ =>
                DesktopLifetime().Windows.OfType<MyDialogWindow>().Should().HaveCount(1));

            var dialogWindow = DesktopLifetime().Windows.OfType<MyDialogWindow>().Single();
            var edit = dialogWindow.GetVisualDescendants().OfType<DialogTodoListEditView>().Single();

            edit.NameTextBox.Focus();
            window.KeyTextInput(name);
            edit.DescriptionTextBox.Text = description;
            Dispatcher.UIThread.RunJobs();

            edit.SaveButton.Focus();
            window.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
            Dispatcher.UIThread.RunJobs();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<TodoList>().ToArray();
                items.Should().HaveCount(DefaultSeeder.TodoLists.Length + 1);
                items.Should().ContainSingle(x => x.Name == name && x.Description == description);
            });
        });

    private static IClassicDesktopStyleApplicationLifetime DesktopLifetime() =>
        (IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!;
}
