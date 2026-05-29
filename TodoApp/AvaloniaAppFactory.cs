using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using TodoApp.Api;
using TodoApp.Infrastructure;

namespace TodoApp;

public static class AvaloniaAppFactory
{
    public static (AppOptions Options, Client ApiClient) CreateClient(string apiUrl)
    {
        var options = new AppOptions { ApiUrl = apiUrl };
        var normalizedUrl = apiUrl.TrimEnd('/');
        var httpClient = new HttpClient(new JHipsterResponseHandler { InnerHandler = new HttpClientHandler() })
        {
            BaseAddress = new Uri(normalizedUrl + "/")
        };
        var apiClient = new Client(normalizedUrl, httpClient);
        return (options, apiClient);
    }

    public static AppBuilder Configure(AppOptions appOptions, Client apiClient) =>
        AppBuilder.Configure(() => new App(appOptions, apiClient, new ClassicDesktopStyleApplicationLifetime()))
            .WithInterFont()
            .LogToTrace();
}
