using System;
using System.Net.Http;
using Avalonia;
using CommandLine;
using TodoApp.Api;
using TodoApp.Infrastructure;

namespace TodoApp;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var appOpts = Parser.Default.ParseArguments<AppOptions>(args).MapResult(o => o, _ => new AppOptions());

        var httpClient = new HttpClient(new JHipsterResponseHandler { InnerHandler = new HttpClientHandler() })
        {
            BaseAddress = new Uri(appOpts.ApiUrl.TrimEnd('/') + "/")
        };

        var apiClient = new Client(appOpts.ApiUrl.TrimEnd('/'), httpClient);

        BuildAvaloniaApp(appOpts, apiClient).StartWithClassicDesktopLifetime(args);
    }

    private static AppBuilder BuildAvaloniaApp(AppOptions appOptions, Client apiClient)
    {
        return AppBuilder.Configure(() => new App(appOptions, apiClient))
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}
