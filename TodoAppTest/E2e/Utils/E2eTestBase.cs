using System.Reflection;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using TodoApp;
using TodoApp.ViewModels.Shell;
using TodoApp.Views.Shell;
using TodoAppApi;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e.Utils;

[NonParallelizable]
public abstract class E2ETestBase
{
    protected MainWindow MainWindow { get; private set; } = null!;

    [SetUp]
    public async Task SetUpAsync()
    {
        ApplySeederIfRequested();
        MainWindow = await OpenAsync().ConfigureAwait(true);
        Dispatcher.UIThread.RunJobs();
    }

    [TearDown]
    public void TearDown()
    {
        ClearDatabase();
        CloseAllWindows();
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

    private void ApplySeederIfRequested()
    {
        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is not string seeder)
            return;

        var db = E2EApiHost.ApiFactory.Services.CreateScope().ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
        seederInstance.Clear(db);
        seederInstance.Seed(db);
    }

    private void ClearDatabase()
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

    private static void CloseAllWindows()
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            if (Application.Current?.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime lifetime)
            {
                foreach (var window in lifetime.Windows.ToList())
                    window.Close();
            }
        });
    }

    private static async Task<MainWindow> OpenAsync()
    {
        var app = (App)Application.Current!;
        var window = CreateShell(app);

        if (app.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = window;

        window.Show();

        if (window.DataContext is MainWindowViewModel viewModel)
            await viewModel.InitializeAsync().ConfigureAwait(true);

        return window;
    }

    private static MainWindow CreateShell(App app)
    {
        var method = app.GetType().GetMethod(
            "CreateShell",
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!;
        return (MainWindow)method.Invoke(app, null)!;
    }
}
