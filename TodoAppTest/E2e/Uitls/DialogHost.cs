using Avalonia.Controls;

namespace TodoAppTest.E2e.Uitls;

public sealed record DialogHost<TDialogWindow, TView>(TDialogWindow Window, TView View)
    where TDialogWindow : Window
    where TView : Control;
