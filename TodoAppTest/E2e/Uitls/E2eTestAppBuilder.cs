using Avalonia;
using Avalonia.Headless;
using TodoAppTest.E2e.Uitls;

[assembly: AvaloniaTestApplication(typeof(TodoAppTest.E2e.E2ETestAppBuilder))]

namespace TodoAppTest.E2e;

public static class E2ETestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        E2ETestRuntime.EnsureInitialized();
        return E2ETestRuntime.CreateAppBuilder();
    }
}
