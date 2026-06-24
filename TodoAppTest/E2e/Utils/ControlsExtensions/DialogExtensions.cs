using Avalonia.Controls;
using Avalonia.VisualTree;

namespace TodoAppTest.E2e.Utils.ControlsExtensions;

public static class DialogExtensions
{
    public static DialogHost<TView> WaitForDialog<TView>(this TopLevel anchor)
        where TView : Control
    {
        E2EEventually.Assert(() => FindDialog<TView>(anchor));
        return FindDialog<TView>(anchor);
    }

    private static DialogHost<TView> FindDialog<TView>(TopLevel anchor)
        where TView : Control
    {
        var view = anchor.GetVisualDescendants().OfType<TView>().Single();
        return new DialogHost<TView>(view);
    }
}

public sealed record DialogHost<TView>(TView View)
    where TView : Control;
