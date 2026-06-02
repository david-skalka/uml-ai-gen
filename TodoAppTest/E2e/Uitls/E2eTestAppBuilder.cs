using Avalonia;
using Avalonia.Headless;
using TodoAppTest.E2e.Uitls;

[assembly: AvaloniaTestApplication(typeof(E2ETestAppBuilder))]

namespace TodoAppTest.E2e.Uitls;

public static class E2ETestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        E2ETestRuntime.EnsureInitialized();
        return E2ETestRuntime.CreateAppBuilder();
    }
}