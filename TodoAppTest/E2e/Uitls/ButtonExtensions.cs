using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;

namespace TodoAppTest.E2e.Uitls;

public static class ButtonExtensions
{
    public static void PerformClick(this Button button) =>
        button.PerformClick(TopLevel.GetTopLevel(button)!);

    public static void PerformClick(this Button button, TopLevel topLevel)
    {
        button.Focus();
        topLevel.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
    }
}
