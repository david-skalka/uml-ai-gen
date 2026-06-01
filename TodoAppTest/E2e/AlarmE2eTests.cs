using System.Globalization;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;
using FluentAssertions;
using TodoApp.Api;
using TodoApp.Views.Alarm;
using TodoApp.Views.Dialogs;
using TodoAppApi;
using TodoAppTest.E2e.Uitls;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

public class AlarmE2ETests : E2ETestBase
{
    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Show()
    {
        var window = await E2ETestRuntime.OpenMainWindowAsync().ConfigureAwait(true);
        window.FindByContent("Alarm").PerformClick();

        var page = window.GetVisualDescendants().OfType<AlarmPageView>().Single();
        var grid = page.AlarmsGrid;

        grid.Should().EventuallySatisfy(
            () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Create()
    {
        var window = await E2ETestRuntime.OpenMainWindowAsync().ConfigureAwait(true);
        window.FindByContent("Alarm").PerformClick();

        var page = window.GetVisualDescendants().OfType<AlarmPageView>().Single();

        page.NewButton.PerformClick();

        var edit = window.WaitForDialog<MyDialogWindow, DialogAlarmEditView>().View;

        edit.TitleTextBox.TypeText("Morning run");
        edit.TimeTextBox.ReplaceText("2026-05-24 07:30");

        edit.SaveButton.PerformClick();

        page.AlarmsGrid.Should().EventuallySatisfy(() =>
        {
            var items = page.AlarmsGrid.ItemsSource!.Cast<Alarm>().ToArray();
            items.Should().HaveCount(DefaultSeeder.Alarms.Length + 1);
        });
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Edit()
    {
        const string title = "Updated";
        const string time = "2026-05-23 09:00";
        var updatedTime = DateTimeOffset.Parse(time, CultureInfo.InvariantCulture);
        var original = DefaultSeeder.Alarms[0];

        var window = await E2ETestRuntime.OpenMainWindowAsync().ConfigureAwait(true);
        window.FindByContent("Alarm").PerformClick();

        var page = window.GetVisualDescendants().OfType<AlarmPageView>().Single();
        var grid = page.AlarmsGrid;

        grid.Should().EventuallySatisfy(
            () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

        grid.SelectedItem = grid.ItemsSource!.Cast<Alarm>().Single(x => x.Id == original.Id);

        page.EditButton.PerformClick();

        var edit = window.WaitForDialog<MyDialogWindow, DialogAlarmEditView>().View;

        edit.TitleTextBox.ReplaceText(title);
        edit.TimeTextBox.ReplaceText(time);

        edit.SaveButton.PerformClick();

        grid.Should().EventuallySatisfy(() =>
        {
            var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
            items.Should().HaveCount(DefaultSeeder.Alarms.Length);
            items.Should().ContainSingle(x => x.Id == original.Id && x.Title == title && x.Time == updatedTime);
        });
    }

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public async Task Delete()
    {
        var original = DefaultSeeder.Alarms[0];

        var window = await E2ETestRuntime.OpenMainWindowAsync().ConfigureAwait(true);
        window.FindByContent("Alarm").PerformClick();

        var page = window.GetVisualDescendants().OfType<AlarmPageView>().Single();
        var grid = page.AlarmsGrid;

        grid.Should().EventuallySatisfy(
            () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

        grid.SelectedItem = grid.ItemsSource!.Cast<Alarm>().Single(x => x.Id == original.Id);

        page.DeleteButton.PerformClick();

        var confirm = window.WaitForDialog<MyDialogWindow, DialogNotificationView>();
        confirm.View.FindByContent("Delete").PerformClick();

        grid.Should().EventuallySatisfy(() =>
        {
            var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
            items.Should().HaveCount(DefaultSeeder.Alarms.Length - 1);
            items.Should().NotContain(x => x.Id == original.Id);
        });
    }
}
