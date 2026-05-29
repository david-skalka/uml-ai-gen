using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Views.Shell;
using TodoAppApi;
using TodoAppTest.Integration.Infrastructure;

namespace TodoAppTest.E2e.Uitls;

public sealed class E2ESession : IDisposable
{
    private static readonly AsyncLocal<E2ESession?> Active = new();

    private HeadlessUnitTestSession? _headless;

    public E2ESession()
    {
        ApiFactory = new CustomWebApplicationFactory<Program>();
        ApiHttpClient = ApiFactory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var baseUrl = ApiHttpClient.BaseAddress!.AbsoluteUri.TrimEnd('/');
        Options = new AppOptions { ApiUrl = "http://localhost" };
        ApiClient = new Client(baseUrl, ApiHttpClient);
    }

    public CustomWebApplicationFactory<Program> ApiFactory { get; }

    private HttpClient ApiHttpClient { get; }

    private AppOptions Options { get; }

    private Client ApiClient { get; }

    public void Start()
    {
        Active.Value = this;
        _headless = HeadlessUnitTestSession.StartNew(
            typeof(AppHostEntry),
            AvaloniaTestIsolationLevel.PerTest);
    }

    public Task RunUiAsync(Func<Task> action) =>
        _headless!.Dispatch(action, CancellationToken.None);

    public Task<T> RunUiAsync<T>(Func<Task<T>> action) =>
        _headless!.Dispatch(action, CancellationToken.None);

    public async Task<MainWindow> OpenMainWindowAsync()
    {
        var app = (App)Application.Current!;
        var window = await app.EnsureMainShellAsync();
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    public void Dispose()
    {
        _headless?.Dispose();
        ApiHttpClient.Dispose();
        ApiFactory.Dispose();
        Active.Value = null;
    }

    private AppBuilder BuildApp() =>
        AvaloniaAppFactory.Configure(Options, ApiClient)
            .UseHeadless(new AvaloniaHeadlessPlatformOptions());

    private sealed class AppHostEntry
    {
        public static AppBuilder BuildAvaloniaApp() =>
            Active.Value!.BuildApp();
    }
}
