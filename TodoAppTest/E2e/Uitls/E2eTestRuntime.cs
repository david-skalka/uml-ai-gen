using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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

public static class E2ETestRuntime
{
    private static CustomWebApplicationFactory<Program>? _apiFactory;
    private static HttpClient? _apiHttpClient;
    private static AppOptions? _options;
    private static Client? _apiClient;
    private static ClassicDesktopStyleApplicationLifetime? _desktopLifetime;

    public static CustomWebApplicationFactory<Program> ApiFactory => _apiFactory!;

    public static void EnsureInitialized()
    {
        if (_apiFactory is not null)
            return;

        _apiFactory = new CustomWebApplicationFactory<Program>();
        _apiHttpClient = _apiFactory.CreateClient(
            new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var myHandler = new JHipsterResponseHandler();
        myHandler.InnerHandler = _apiFactory.Server.CreateHandler();

        var apiUrl = "http://localhost";
        _options = new AppOptions { ApiUrl = apiUrl };
        var normalizedUrl = apiUrl.TrimEnd('/');
        var httpClient = new HttpClient(myHandler)
        {
            BaseAddress = _apiFactory.ClientOptions.BaseAddress
        };

        _apiClient = new Client(normalizedUrl, httpClient);

        InitializeAvalonia();
    }

    private static void InitializeAvalonia()
    {
        AppBuilder.Configure(() => new App(_options!, _apiClient!, new ClassicDesktopStyleApplicationLifetime()))
            .WithInterFont()
            .LogToTrace()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = true });

    }

    public static AppBuilder CreateAppBuilder()
    {
        return AppBuilder.Configure(() => new App(_options!, _apiClient!, new ClassicDesktopStyleApplicationLifetime()))
            .WithInterFont()
            .LogToTrace()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = true });
    }

    public static void DisposeCurrent()
    {
        if (_desktopLifetime is not null)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                var activeWindows = _desktopLifetime.Windows.ToList();
                foreach (var window in activeWindows)
                {
                    window.Close();
                }
                _desktopLifetime.Shutdown();
            });
            
            _desktopLifetime = null;
        }

        _apiHttpClient?.Dispose();
        _apiFactory?.Dispose();
        _apiHttpClient = null;
        _apiFactory = null;
        _apiClient = null;
        _options = null;
    }


    public static async Task<MainWindow> OpenMainWindowAsync()
    {
        var app = (App)Application.Current!;
        var window = await app.EnsureMainShellAsync().ConfigureAwait(true);

        if (_desktopLifetime is not null)
            _desktopLifetime.MainWindow = window;
            
        Dispatcher.UIThread.RunJobs();
        return window;
    }
}