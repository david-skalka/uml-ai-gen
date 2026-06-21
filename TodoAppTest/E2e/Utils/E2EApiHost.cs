using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using TodoApp;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoAppApi;
using TodoAppTest.Integration.Infrastructure;

[assembly: AvaloniaTestApplication(typeof(TodoAppTest.E2e.Utils.AvalaniaInitializer))]

namespace TodoAppTest.E2e.Utils;

public static class E2EApiHost
{
    public static CustomWebApplicationFactory<Program> ApiFactory { get; private set; } = null!;
    public static Client ApiClient { get; private set; } = null!;
    public static AppOptions Options { get; private set; } = null!;

    public static void Create()
    {
        var apiFactory = new CustomWebApplicationFactory<Program>();

        var handler = new JHipsterResponseHandler
        {
            InnerHandler = apiFactory.Server.CreateHandler()
        };

        const string apiUrl = "http://localhost";
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = apiFactory.ClientOptions.BaseAddress
        };

        var options = new AppOptions { ApiUrl = apiUrl };
        var apiClient = new Client(apiUrl.TrimEnd('/'), httpClient);

        ApiFactory = apiFactory;
        ApiClient = apiClient;
        Options = options;
    }

    public static async ValueTask DisposeAsync() =>
        await ApiFactory.DisposeAsync();
}

public static class AvalaniaInitializer
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder
        .Configure(() => new App(E2EApiHost.Options, E2EApiHost.ApiClient, new ClassicDesktopStyleApplicationLifetime()))
        .WithInterFont()
        .LogToTrace()
        .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = true });
}
