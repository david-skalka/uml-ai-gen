using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Microsoft.Extensions.DependencyInjection;
using TodoApp;
using TodoApp.Views.Shell;
using TodoAppApi;
using TodoAppTest.Integration.Seeders;

namespace TodoAppTest.E2e.Utils;

[NonParallelizable]
public abstract class E2ETestBase
{
    private E2EHostState Host => E2EHost.State;

    [SetUp]
    public void SetUp() =>
        ApplySeederIfRequested();

    [TearDown]
    public void TearDown()
    {
        ClearDatabase();
        CloseAllWindows();
    }

    protected async Task<MainWindow> OpenMainWindowAsync()
    {
        var app = (App)Application.Current!;
        var window = await app.EnsureMainShellAsync().ConfigureAwait(true);
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    private void ApplySeederIfRequested()
    {
        if (TestContext.CurrentContext.Test.Properties.Get("Seeder") is not string seeder)
            return;

        var db = Host.ApiFactory.Services.CreateScope().ServiceProvider
            .GetRequiredService<ApplicationDbContext>();
        var seederInstance = (ISeeder)Activator.CreateInstance(Type.GetType(seeder)!)!;
        seederInstance.Clear(db);
        seederInstance.Seed(db);
    }

    private void ClearDatabase()
    {
        var db = Host.ApiFactory.Services.CreateScope().ServiceProvider
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
            if (Application.Current?.ApplicationLifetime
                    is ClassicDesktopStyleApplicationLifetime lifetime)
            {
                foreach (var w in lifetime.Windows.ToList())
                    w.Close();
            }
        });
    }
}
