using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using FluentAssertions;
using TodoApp.Api;
using TodoApp.Views.Dialogs;
using TodoApp.Views.TodoList;
using TodoAppApi;
using TodoAppTest.E2e.Uitls;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

public class TodoListE2ETests : E2eTestBase
{
    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Show() =>
        E2eTestRuntime.RunOnPageAsync<TodoListPageView>(host =>
        {
            var grid = host.Page.TodoListsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));

            return Task.CompletedTask;
        });

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Create() =>
        E2eTestRuntime.RunOnPageAsync<TodoListPageView>(host =>
        {
            host.Page.NewButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            var edit = host.Window.WaitForDialog<MyDialogWindow, DialogTodoListEditView>().View;

            edit.NameTextBox.TypeText("Shopping");
            edit.DescriptionTextBox.TypeText("Groceries");

            edit.SaveButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            host.Page.TodoListsGrid.Should().EventuallySatisfy(() =>
            {
                var items = host.Page.TodoListsGrid.ItemsSource!.Cast<TodoList>().ToArray();
                
                items.Should().HaveCount(DefaultSeeder.TodoLists.Length+1);
            });

            return Task.CompletedTask;
        });

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Edit() =>
        E2eTestRuntime.RunOnPageAsync<TodoListPageView>(host =>
        {
            const string name = "Updated";
            const string description = "After";
            var original = DefaultSeeder.TodoLists[0];

            var grid = host.Page.TodoListsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));

            grid.SelectedItem = grid.ItemsSource!.Cast<TodoList>().Single(x => x.Id == original.Id);
            Dispatcher.UIThread.RunJobs();

            host.Page.EditButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            var edit = host.Window.WaitForDialog<MyDialogWindow, DialogTodoListEditView>().View;

            edit.NameTextBox.ReplaceText(name);
            edit.DescriptionTextBox.ReplaceText(description);

            edit.SaveButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<TodoList>().ToArray();
                items.Should().HaveCount(DefaultSeeder.TodoLists.Length);
                items.Should().ContainSingle(x => x.Id == original.Id && x.Name == name && x.Description == description);
            });

            return Task.CompletedTask;
        });

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Delete() =>
        E2eTestRuntime.RunOnPageAsync<TodoListPageView>(host =>
        {
            var original = DefaultSeeder.TodoLists[0];
            var grid = host.Page.TodoListsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));

            grid.SelectedItem = grid.ItemsSource!.Cast<TodoList>().Single(x => x.Id == original.Id);
            Dispatcher.UIThread.RunJobs();

            host.Page.DeleteButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            var confirm = host.Window.WaitForDialog<MyDialogWindow, DialogNotificationView>();
            confirm.View.FindByContent("Delete").PerformClick();
            Dispatcher.UIThread.RunJobs();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<TodoList>().ToArray();
                items.Should().HaveCount(DefaultSeeder.TodoLists.Length - 1);
                items.Should().NotContain(x => x.Id == original.Id);
            });

            return Task.CompletedTask;
        });

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task GroupByName() =>
        E2eTestRuntime.RunOnPageAsync<TodoListPageView>(host =>
        {
            host.Page.MainTabControl.SelectedItem = host.Page.ExtraActionsTab;
            Dispatcher.UIThread.RunJobs();

            host.Page.GroupByNameRunButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            host.Page.GroupByNameResultsGrid.Should().EventuallySatisfy(() =>
            {
                var items = host.Page.GroupByNameResultsGrid.ItemsSource!.Cast<GroupByNameOutput>().ToArray();
                items.Should().HaveCount(DefaultSeeder.TodoLists.Length);
            });

            return Task.CompletedTask;
        });
}
