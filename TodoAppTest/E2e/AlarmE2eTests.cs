using System.Globalization;
using Avalonia;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAssertions;
using TodoApp.Api;
using TodoApp.Views.Alarm;
using TodoApp.Views.Dialogs;
using TodoAppApi;
using TodoAppTest.E2e.Uitls;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

public class AlarmE2ETests : E2eTestBase
{
    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Show() =>
        RunOnAlarmPageAsync(host =>
        {
            var grid = host.Page.AlarmsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

            return Task.CompletedTask;
        });

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Create() =>
        RunOnAlarmPageAsync(host =>
        {
            Dispatcher.UIThread.RunJobs();
            host.Page.NewButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            var edit = host.Window.WaitForDialog<MyDialogWindow, DialogAlarmEditView>().View;

            edit.TitleTextBox.TypeText("Morning run");
            edit.TimeTextBox.ReplaceText("2026-05-24 07:30");

            edit.SaveButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            host.Page.AlarmsGrid.Should().EventuallySatisfy(() =>
            {
                var items = host.Page.AlarmsGrid.ItemsSource!.Cast<Alarm>().ToArray();
                items.Should().HaveCount(DefaultSeeder.Alarms.Length + 1);
            });

            return Task.CompletedTask;
        });




    
    
    

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Edit() =>
        RunOnAlarmPageAsync(host =>
        {
            const string title = "Updated";
            const string time = "2026-05-23 09:00";
            var updatedTime = DateTimeOffset.Parse(time, CultureInfo.InvariantCulture);
            var original = DefaultSeeder.Alarms[0];

            var grid = host.Page.AlarmsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

            grid.SelectedItem = grid.ItemsSource!.Cast<Alarm>().Single(x => x.Id == original.Id);
            Dispatcher.UIThread.RunJobs();

            host.Page.EditButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            var edit = host.Window.WaitForDialog<MyDialogWindow, DialogAlarmEditView>().View;

            edit.TitleTextBox.ReplaceText(title);
            edit.TimeTextBox.ReplaceText(time);

            edit.SaveButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
                items.Should().HaveCount(DefaultSeeder.Alarms.Length);
                items.Should().ContainSingle(x => x.Id == original.Id && x.Title == title && x.Time == updatedTime);
            });

            return Task.CompletedTask;
        });

    [AvaloniaTest]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Delete() =>
        RunOnAlarmPageAsync(host =>
        {
            var original = DefaultSeeder.Alarms[0];
            var grid = host.Page.AlarmsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

            grid.SelectedItem = grid.ItemsSource!.Cast<Alarm>().Single(x => x.Id == original.Id);
            Dispatcher.UIThread.RunJobs();

            host.Page.DeleteButton.PerformClick();
            Dispatcher.UIThread.RunJobs();

            var confirm = host.Window.WaitForDialog<MyDialogWindow, DialogNotificationView>();
            confirm.View.FindByContent("Delete").PerformClick();
            Dispatcher.UIThread.RunJobs();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
                items.Should().HaveCount(DefaultSeeder.Alarms.Length - 1);
                items.Should().NotContain(x => x.Id == original.Id);
            });

            return Task.CompletedTask;
        });

    private static Task RunOnAlarmPageAsync(Func<PageHost<AlarmPageView>, Task> action) =>
        E2eTestRuntime.RunUiAsync(async () =>
        {
            var window = await E2eTestRuntime.OpenMainWindowAsync().ConfigureAwait(true);
            window.FindByContent("Alarm").PerformClick();
            Dispatcher.UIThread.RunJobs();
            var page = window.GetVisualDescendants().OfType<AlarmPageView>().Single();
            
            await action(new PageHost<AlarmPageView>(window, page)).ConfigureAwait(true);
        });
}
