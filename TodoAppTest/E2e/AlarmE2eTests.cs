using System.Globalization;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;
using FluentAssertions;
using TodoApp.Api;
using TodoApp.Views.Alarm;
using TodoApp.Views.Dialogs;
using TodoAppTest.E2e.Utils;
using TodoAppTest.E2e.Utils.ControlsExtensions;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

[Category("E2e")]
public class AlarmE2ETests : E2ETestBase
{
    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Show()
    {
        MainWindow.FindByContent("Alarm").PerformClick();

        var page = MainWindow.GetVisualDescendants().OfType<AlarmPageView>().Single();
        var grid = page.AlarmsGrid;

        E2EEventually.Assert(() =>
            grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Create()
    {
        MainWindow.FindByContent("Alarm").PerformClick();

        var page = MainWindow.GetVisualDescendants().OfType<AlarmPageView>().Single();

        page.NewButton.PerformClick();

        var edit = MainWindow.WaitForDialog<DialogAlarmEditView>().View;

        edit.TitleTextBox.TypeText("Morning run");
        edit.TimeTextBox.ReplaceText("2026-05-24 07:30");

        edit.SaveButton.PerformClick();

        E2EEventually.Assert(() =>
        {
            var items = page.AlarmsGrid.ItemsSource!.Cast<Alarm>().ToArray();
            items.Should().HaveCount(DefaultSeeder.Alarms.Length + 1);
        });
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Edit()
    {
        const string title = "Updated";
        const string time = "2026-05-23 09:00";
        var updatedTime = DateTimeOffset.Parse(time, CultureInfo.InvariantCulture);
        var original = DefaultSeeder.Alarms[0];

        MainWindow.FindByContent("Alarm").PerformClick();

        var page = MainWindow.GetVisualDescendants().OfType<AlarmPageView>().Single();
        var grid = page.AlarmsGrid;

        E2EEventually.Assert(() =>
            grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

        grid.SelectedItem = grid.ItemsSource!.Cast<Alarm>().Single(x => x.Id == original.Id);

        page.EditButton.PerformClick();

        var edit = MainWindow.WaitForDialog<DialogAlarmEditView>().View;

        edit.TitleTextBox.ReplaceText(title);
        edit.TimeTextBox.ReplaceText(time);

        edit.SaveButton.PerformClick();

        E2EEventually.Assert(() =>
        {
            var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
            items.Should().HaveCount(DefaultSeeder.Alarms.Length);
            items.Should().ContainSingle(x => x.Id == original.Id && x.Title == title && x.Time == updatedTime);
        });
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public void Delete()
    {
        var original = DefaultSeeder.Alarms[0];

        MainWindow.FindByContent("Alarm").PerformClick();

        var page = MainWindow.GetVisualDescendants().OfType<AlarmPageView>().Single();
        var grid = page.AlarmsGrid;

        E2EEventually.Assert(() =>
            grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

        grid.SelectedItem = grid.ItemsSource!.Cast<Alarm>().Single(x => x.Id == original.Id);

        page.DeleteButton.PerformClick();

        var confirm = MainWindow.WaitForDialog<DialogNotificationView>();
        confirm.View.FindByContent("Delete").PerformClick();

        E2EEventually.Assert(() =>
        {
            var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
            items.Should().HaveCount(DefaultSeeder.Alarms.Length - 1);
            items.Should().NotContain(x => x.Id == original.Id);
        });
    }
}
