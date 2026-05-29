using Avalonia.Controls;
using Avalonia.VisualTree;
using FluentAssertions;

namespace TodoAppTest.E2e.Uitls;

public static class DialogExtensions
{
    public static DialogHost<TDialogWindow, TView> WaitForDialog<TDialogWindow, TView>(this TopLevel anchor)
        where TDialogWindow : Window
        where TView : Control
    {
        anchor.Should().EventuallySatisfy(_ => FindDialog<TDialogWindow, TView>());
        return FindDialog<TDialogWindow, TView>();
    }

    public static TView WaitForDialogView<TView>(this TopLevel anchor)
        where TView : Control
    {
        anchor.Should().EventuallySatisfy(_ => FindDialogView<TView>());
        return FindDialogView<TView>();
    }

    public static DialogHost<TDialogWindow, TView> FindDialog<TDialogWindow, TView>()
        where TDialogWindow : Window
        where TView : Control
    {
        var window = E2ESession.DesktopLifetime.Windows.OfType<TDialogWindow>().Single();
        var view = window.GetVisualDescendants().OfType<TView>().Single();
        return new DialogHost<TDialogWindow, TView>(window, view);
    }

    public static TView FindDialogView<TView>()
        where TView : Control =>
        E2ESession.DesktopLifetime.Windows
            .SelectMany(w => w.GetVisualDescendants())
            .OfType<TView>()
            .Single();
}
