using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Ursa.Controls;

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

    public static void NavigateByHeader(this Control root, string header)
    {
        var item = root.GetVisualDescendants().OfType<NavMenuItem>()
            .Single(i => i.Header?.ToString() == header);
        item.Command!.Execute(item.CommandParameter);
        Dispatcher.UIThread.RunJobs();
    }
}
