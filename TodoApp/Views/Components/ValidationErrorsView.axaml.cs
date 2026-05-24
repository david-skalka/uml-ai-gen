using Avalonia;
using Avalonia.Controls;

namespace TodoApp.Views.Components;

public partial class ValidationErrorsView : UserControl
{
    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<ValidationErrorsView, string>(nameof(Text));

    public ValidationErrorsView()
    {
        InitializeComponent();
    }

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
}
