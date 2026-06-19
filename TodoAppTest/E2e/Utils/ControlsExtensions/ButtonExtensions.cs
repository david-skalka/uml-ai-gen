using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace TodoAppTest.E2e.Utils.ControlsExtensions;

public static class ButtonExtensions
{
    public static void PerformClick(this Button button)
    {
        button.PerformClick(TopLevel.GetTopLevel(button)!);
    }

    private static void PerformClick(this Button button, TopLevel topLevel)
    {
        button.Focus();
        topLevel.KeyReleaseQwerty(PhysicalKey.Space, RawInputModifiers.None);
        Dispatcher.UIThread.RunJobs();
    }

    public static Button FindByContent(this Control root, string content)
    {
        return root.GetVisualDescendants().OfType<Button>().Single(b => b.Content?.ToString() == content);
    }
}