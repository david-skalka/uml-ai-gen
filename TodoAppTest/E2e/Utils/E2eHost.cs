using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Headless;
using TodoApp;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoAppApi;
using TodoAppTest.E2e.Utils;
using TodoAppTest.Integration.Infrastructure;

[assembly: AvaloniaTestApplication(typeof(E2EHost))]

namespace TodoAppTest.E2e.Utils;

public static class E2EHost
{
    private static E2EHostState _state = null!;

    internal static E2EHostState State => _state ??= E2EHostState.Create();

    public static AppBuilder BuildAvaloniaApp() => State.BuildApp();

    internal static void Reset() =>
        _state = null!;
}

[SetUpFixture]
public sealed class E2EAssemblyTeardown
{
    [OneTimeTearDown]
    public async Task TearDownAsync()
    {
        await E2EHost.State.DisposeAsync();
        E2EHost.Reset();
    }
}

internal sealed class E2EHostState : IAsyncDisposable
{
    public CustomWebApplicationFactory<Program> ApiFactory { get; }
    public Client ApiClient { get; }
    public AppOptions Options { get; }

    private E2EHostState(
        CustomWebApplicationFactory<Program> apiFactory,
        Client apiClient,
        AppOptions options)
    {
        ApiFactory = apiFactory;
        ApiClient = apiClient;
        Options = options;
    }

    public static E2EHostState Create()
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

        return new E2EHostState(apiFactory, apiClient, options);
    }

    public AppBuilder BuildApp() =>
        AppBuilder
            .Configure(() => new App(Options, ApiClient, new ClassicDesktopStyleApplicationLifetime()))
            .WithInterFont()
            .LogToTrace()
            .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = true });

    public async ValueTask DisposeAsync() =>
        await ApiFactory.DisposeAsync();
}
