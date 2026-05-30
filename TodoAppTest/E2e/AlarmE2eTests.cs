using System.Globalization;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Api;
using TodoApp.Views.Alarm;
using TodoApp.Views.Dialogs;
using TodoAppApi;
using TodoAppTest.E2e.Uitls;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e;

[Category("E2e")]
[NonParallelizable]
public class AlarmE2ETests
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
        RunOnAlarmPageAsync(host =>
        {
            var grid = host.Page.AlarmsGrid;

            grid.Should().EventuallySatisfy(
                () => grid.ItemsSource.Cast<object>().Should().HaveCount(DefaultSeeder.Alarms.Length));

            return Task.CompletedTask;
        });

    [Test]
    [Property("Seeder", "TodoAppTest.Integration.Seeders.DefaultSeeder")]
    public Task Create() =>
        RunOnAlarmPageAsync(host =>
        {
            host.Page.NewButton.PerformClick();

            var edit = host.Window.WaitForDialog<MyDialogWindow, DialogAlarmEditView>().View;

            edit.TitleTextBox.TypeText("Morning run");
            edit.TimeTextBox.ReplaceText("2026-05-24 07:30");

            edit.SaveButton.PerformClick();

            host.Page.AlarmsGrid.Should().EventuallySatisfy(() =>
            {
                var items = host.Page.AlarmsGrid.ItemsSource!.Cast<Alarm>().ToArray();
                items.Should().HaveCount(DefaultSeeder.Alarms.Length + 1);
            });

            return Task.CompletedTask;
        });

    [Test]
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

            var edit = host.Window.WaitForDialog<MyDialogWindow, DialogAlarmEditView>().View;

            edit.TitleTextBox.ReplaceText(title);
            edit.TimeTextBox.ReplaceText(time);

            edit.SaveButton.PerformClick();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
                items.Should().HaveCount(DefaultSeeder.Alarms.Length);
                items.Should().ContainSingle(x => x.Id == original.Id && x.Title == title && x.Time == updatedTime);
            });

            return Task.CompletedTask;
        });

    [Test]
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

            var confirm = host.Window.WaitForDialog<MyDialogWindow, DialogNotificationView>();
            confirm.View.FindByContent("Delete").PerformClick();

            grid.Should().EventuallySatisfy(() =>
            {
                var items = grid.ItemsSource!.Cast<Alarm>().ToArray();
                items.Should().HaveCount(DefaultSeeder.Alarms.Length - 1);
                items.Should().NotContain(x => x.Id == original.Id);
            });

            return Task.CompletedTask;
        });

    private Task RunOnAlarmPageAsync(Func<PageHost<AlarmPageView>, Task> action) =>
        _session!.RunUiAsync(async () =>
        {
            var window = await _session.OpenMainWindowAsync();
            window.FindByContent("Alarm").PerformClick();
            Dispatcher.UIThread.RunJobs();
            var page = window.GetVisualDescendants().OfType<AlarmPageView>().Single();
            await action(new PageHost<AlarmPageView>(window, page));
        });
}
