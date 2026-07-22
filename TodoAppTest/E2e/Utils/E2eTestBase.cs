using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using TodoApp;
using TodoApp.ViewModels.Shell;
using TodoApp.Views.Shell;
using TodoApp.Views.TodoList;
using TodoAppApi;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e.Utils;

[NonParallelizable]
public abstract class E2ETestBase
{
    protected MainWindow MainWindow { get; private set; } = null!;

    [SetUp]
    public void SetUp()
    {
        ApplySeederIfRequested();
        MainWindow = Open();
        Dispatcher.UIThread.RunJobs();
    }

    [TearDown]
    public void TearDown()
    {
        ClearDatabase();
    }

    [OneTimeSetUp]
    public void SetUpHost()
    {
        E2EApiHost.Create();
    }

    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await E2EApiHost.DisposeAsync();
    }

    private static void ApplySeederIfRequested()
    {
        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is not string seeder)
            return;

        var db = E2EApiHost.ApiFactory.Services.CreateScope().ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
        seederInstance.Clear(db);
        seederInstance.Seed(db);
    }

    private static void ClearDatabase()
    {
        var db = E2EApiHost.ApiFactory.Services.CreateScope().ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is string seeder)
        {
            var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
            seederInstance.Clear(db);
        }
        else
        {
            new DefaultSeeder().Clear(db);
        }
    }

    private static MainWindow Open()
    {
        var app = (App)Application.Current!;
        var window = (MainWindow)app.MainWindow;
        window.Show();

        if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = window;

        if (window.DataContext is MainWindowViewModel viewModel)
            viewModel.NavigateCommand.Execute(nameof(TodoListPageView));

        return window;
    }
}
