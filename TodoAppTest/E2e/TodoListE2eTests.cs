using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;
using FluentAssertions;
using TodoApp.Api;
using TodoApp.Views.Dialogs;
using TodoApp.Views.TodoList;
using TodoAppTest.E2e.Utils;
using TodoAppTest.E2e.Utils.ControlsExtensions;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

[Category("E2e")]
public class TodoListE2ETests : E2ETestBase
{
    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Show()
    {
        var page = MainWindow.GetVisualDescendants().OfType<TodoListPageView>().Single();
        var grid = page.TodoListsGrid;

        E2EEventually.Assert(() =>
            grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Create()
    {
        var page = MainWindow.GetVisualDescendants().OfType<TodoListPageView>().Single();

        page.NewButton.PerformClick();

        var edit = MainWindow.WaitForDialog<MyDialogWindow, DialogTodoListEditView>().View;

        edit.NameTextBox.TypeText("Shopping");
        edit.DescriptionTextBox.TypeText("Groceries");

        edit.SaveButton.PerformClick();

        E2EEventually.Assert(() =>
        {
            var items = page.TodoListsGrid.ItemsSource!.Cast<TodoList>().ToArray();

            items.Should().HaveCount(DefaultSeeder.TodoLists.Length + 1);
        });
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Edit()
    {
        const string name = "Updated";
        const string description = "After";
        var original = DefaultSeeder.TodoLists[0];

        var page = MainWindow.GetVisualDescendants().OfType<TodoListPageView>().Single();
        var grid = page.TodoListsGrid;

        E2EEventually.Assert(() =>
            grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));

        grid.SelectedItem = grid.ItemsSource!.Cast<TodoList>().Single(x => x.Id == original.Id);

        page.EditButton.PerformClick();

        var edit = MainWindow.WaitForDialog<MyDialogWindow, DialogTodoListEditView>().View;

        edit.NameTextBox.ReplaceText(name);
        edit.DescriptionTextBox.ReplaceText(description);

        edit.SaveButton.PerformClick();

        E2EEventually.Assert(() =>
        {
            var items = grid.ItemsSource!.Cast<TodoList>().ToArray();
            items.Should().HaveCount(DefaultSeeder.TodoLists.Length);
            items.Should().ContainSingle(x => x.Id == original.Id && x.Name == name && x.Description == description);
        });
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Delete()
    {
        var original = DefaultSeeder.TodoLists[0];

        var page = MainWindow.GetVisualDescendants().OfType<TodoListPageView>().Single();
        var grid = page.TodoListsGrid;

        E2EEventually.Assert(() =>
            grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.TodoLists.Length));

        grid.SelectedItem = grid.ItemsSource!.Cast<TodoList>().Single(x => x.Id == original.Id);

        page.DeleteButton.PerformClick();

        var confirm = MainWindow.WaitForDialog<MyDialogWindow, DialogNotificationView>();
        confirm.View.FindByContent("Delete").PerformClick();

        E2EEventually.Assert(() =>
        {
            var items = grid.ItemsSource!.Cast<TodoList>().ToArray();
            items.Should().HaveCount(DefaultSeeder.TodoLists.Length - 1);
            items.Should().NotContain(x => x.Id == original.Id);
        });
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void GroupByName()
    {
        var page = MainWindow.GetVisualDescendants().OfType<TodoListPageView>().Single();

        page.MainTabControl.SelectedItem = page.ExtraActionsTab;

        page.GroupByNameRunButton.PerformClick();

        E2EEventually.Assert(() =>
        {
            var items = page.GroupByNameResultsGrid.ItemsSource!.Cast<GroupByNameOutput>().ToArray();
            items.Should().HaveCount(DefaultSeeder.TodoLists.Length);
        });
    }
}
