using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using Avalonia.VisualTree;
using FluentAssertions;
using TodoApp;

namespace TodoAppTest.E2e.Uitls;

public static class DialogExtensions
{
    public static DialogHost<TDialogWindow, TView> WaitForDialog<TDialogWindow, TView>(this TopLevel anchor)
        where TDialogWindow : Window
        where TView : Control
    {
        anchor.Should().EventuallySatisfy(_ => FindDialog<TDialogWindow, TView>(anchor));
        return FindDialog<TDialogWindow, TView>(anchor);
    }

    public static TView WaitForDialogView<TView>(this TopLevel anchor)
        where TView : Control
    {
        anchor.Should().EventuallySatisfy(_ => FindDialogView<TView>(anchor));
        return FindDialogView<TView>(anchor);
    }

    public static DialogHost<TDialogWindow, TView> FindDialog<TDialogWindow, TView>(TopLevel anchor)
        where TDialogWindow : Window
        where TView : Control
    {
        var window = FindDialogWindow<TDialogWindow>(anchor);
        var view = window.GetVisualDescendants().OfType<TView>().Single();
        return new DialogHost<TDialogWindow, TView>(window, view);
    }

    public static TView FindDialogView<TView>(TopLevel anchor)
        where TView : Control =>
        FindDialogWindow<Window>(anchor)
            .GetVisualDescendants()
            .OfType<TView>()
            .Single();

    private static TDialogWindow FindDialogWindow<TDialogWindow>(TopLevel anchor)
        where TDialogWindow : Window
    {
        
        Dispatcher.UIThread.RunJobs();
        
        
        var container = ContainerLocator.Container;
    
        var tracker = container.Resolve<IActiveDialogTracker>();
        
        return (TDialogWindow)tracker.ActiveDialog!;
        
    }

    private static IEnumerable<Window> GetTopLevels(Window anchorWindow)
    {
        var windows = new HashSet<Window> { anchorWindow };

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            foreach (var window in desktop.Windows.OfType<Window>())
                windows.Add(window);

        return windows;
    }

    private static IEnumerable<Window> CollectOwnedWindows(Window window)
    {
        yield return window;
        foreach (var owned in window.OwnedWindows)
            foreach (var nested in CollectOwnedWindows(owned))
                yield return nested;
    }
}
