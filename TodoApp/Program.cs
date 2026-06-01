using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
            BaseAddress = new Uri(appOpts.ApiUrl)
        };
        var apiClient = new Client(appOpts.ApiUrl, httpClient);

        AppBuilder.Configure(() => new App(appOpts, apiClient, new ClassicDesktopStyleApplicationLifetime()))
            .WithInterFont()
            .LogToTrace()
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);
    }
}
