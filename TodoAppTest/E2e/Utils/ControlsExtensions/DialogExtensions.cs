using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.VisualTree;
using TodoApp.Services;

namespace TodoAppTest.E2e.Utils.ControlsExtensions;

public static class DialogExtensions
{
    public static DialogHost<TView> WaitForDialog<TDialogWindow, TView>(this TopLevel anchor)
        where TDialogWindow : Window
        where TView : Control
    {
        E2EEventually.Assert(() => FindDialog<TDialogWindow, TView>(anchor));
        return FindDialog<TDialogWindow, TView>(anchor);
    }


    private static DialogHost<TView> FindDialog<TDialogWindow, TView>(TopLevel anchor)
        where TDialogWindow : Window
        where TView : Control
    {
        var window = FindDialogWindow<TDialogWindow>(anchor);
        var view = window.GetVisualDescendants().OfType<TView>().Single();
        return new DialogHost<TView>(view);
    }


    private static TDialogWindow FindDialogWindow<TDialogWindow>(TopLevel _)
        where TDialogWindow : Window
    {
        Dispatcher.UIThread.RunJobs();


        var container = ContainerLocator.Container;

        var tracker = container.Resolve<IActiveDialogTracker>();

        return (TDialogWindow)tracker.ActiveDialog!;
    }
}


public sealed record DialogHost<TView>(TView View)
    where TView : Control;
