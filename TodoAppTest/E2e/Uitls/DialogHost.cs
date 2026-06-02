using Avalonia.Controls;

namespace TodoAppTest.E2e.Uitls;

public sealed record DialogHost<TView>(TView View)
    where TView : Control;