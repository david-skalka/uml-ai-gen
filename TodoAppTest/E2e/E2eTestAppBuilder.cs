using Avalonia;
using Avalonia.Headless;
using TodoAppTest.E2e.Uitls;

[assembly: AvaloniaTestApplication(typeof(TodoAppTest.E2e.E2eTestAppBuilder))]

namespace TodoAppTest.E2e;

public static class E2eTestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp()
    {
        E2eTestRuntime.EnsureInitialized();
        return E2eTestRuntime.CreateAppBuilder();
    }
}
