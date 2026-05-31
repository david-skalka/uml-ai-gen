using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Microsoft.AspNetCore.Mvc.Testing;
using TodoApp;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Views.Shell;
using TodoAppApi;
using TodoAppTest.Integration.Infrastructure;

namespace TodoAppTest.E2e.Uitls;

public static class E2eTestRuntime
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

        // Inicializujeme Avalonia aplikaci s vlastním lifetime, abychom ho měli plně pod kontrolou
        InitializeAvalonia();
    }

    private static void InitializeAvalonia()
    {
        _desktopLifetime = new ClassicDesktopStyleApplicationLifetime
        {
            Args = Array.Empty<string>(),
            // Klíčové: Chceme okna a aplikaci zavírat explicitně v Dispose, 
            // ne automaticky při zavření jednoho (např. dialogového) okna.
            ShutdownMode = ShutdownMode.OnExplicitShutdown
        };

        AvaloniaAppFactory.Configure(_options!, _apiClient!)
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = true });

    }

    // Builder necháváme, pokud ho potřebuješ pro nějaké specifické konfigurace, 
    // ale Setup už se děje uvnitř EnsureInitialized
    public static AppBuilder CreateAppBuilder()
    {
        return AvaloniaAppFactory.Configure(_options!, _apiClient!)
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = true });
    }

    public static Task RunUiAsync(Func<Task> action) =>
        Dispatcher.UIThread.InvokeAsync(action);

    public static void DisposeCurrent()
    {
        // 1. Nejprve musíme bezpečně zlikvidovat UI na UI Vlákně
        if (_desktopLifetime is not null)
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                // Zavřeme natvrdo všechna otevřená okna i dialogy, které zůstaly vyset
                var activeWindows = _desktopLifetime.Windows.ToList();
                foreach (var window in activeWindows)
                {
                    window.Close();
                }

                // Vypneme lifetime smyčku
                _desktopLifetime.Shutdown();
            });
            
            _desktopLifetime = null;
        }

        // 2. Vyčištění API a HTTP infrastuktury
        _apiHttpClient?.Dispose();
        _apiFactory?.Dispose();
        _apiHttpClient = null;
        _apiFactory = null;
        _apiClient = null;
        _options = null;
    }

    public static IClassicDesktopStyleApplicationLifetime DesktopLifetime =>
        _desktopLifetime ?? throw new InvalidOperationException("Avalonia runtime is not initialized.");

    public static async Task<MainWindow> OpenMainWindowAsync()
    {
        var app = (App)Application.Current!;
        var window = await app.EnsureMainShellAsync().ConfigureAwait(true);

        if (_desktopLifetime is not null)
            _desktopLifetime.MainWindow = window;
            
        Dispatcher.UIThread.RunJobs();
        return window;
    }

    public static async Task<PageHost<TPage>> OpenPageAsync<TPage>()
        where TPage : Control
    {
        var window = await OpenMainWindowAsync().ConfigureAwait(true);
        var page = window.GetVisualDescendants().OfType<TPage>().Single();
        return new PageHost<TPage>(window, page);
    }

    public static Task RunOnPageAsync<TPage>(Func<PageHost<TPage>, Task> action)
        where TPage : Control =>
        RunUiAsync(async () =>
            await action(await OpenPageAsync<TPage>().ConfigureAwait(true)).ConfigureAwait(true));
}