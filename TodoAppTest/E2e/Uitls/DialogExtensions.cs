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
        anchor.Should().EventuallySatisfy(() => FindDialog<TDialogWindow, TView>(anchor));
        return FindDialog<TDialogWindow, TView>(anchor);
    }


    private static DialogHost<TDialogWindow, TView> FindDialog<TDialogWindow, TView>(TopLevel anchor)
        where TDialogWindow : Window
        where TView : Control
    {
        var window = FindDialogWindow<TDialogWindow>(anchor);
        var view = window.GetVisualDescendants().OfType<TView>().Single();
        return new DialogHost<TDialogWindow, TView>(window, view);
    }


    private static TDialogWindow FindDialogWindow<TDialogWindow>(TopLevel anchor)
        where TDialogWindow : Window
    {
        
        Dispatcher.UIThread.RunJobs();
        
        
        var container = ContainerLocator.Container;
    
        var tracker = container.Resolve<IActiveDialogTracker>();
        
        return (TDialogWindow)tracker.ActiveDialog!;
        
    }


}
