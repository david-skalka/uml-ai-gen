using Avalonia;
using CommandLine;
using TodoApp.Infrastructure;

namespace TodoApp;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        var appOpts = Parser.Default.ParseArguments<AppOptions>(args).MapResult(o => o, _ => new AppOptions());
        var (_, apiClient) = AvaloniaAppFactory.CreateClient(appOpts.ApiUrl);

        AvaloniaAppFactory.Configure(appOpts, apiClient)
            .UsePlatformDetect()
            .StartWithClassicDesktopLifetime(args);
    }
}
