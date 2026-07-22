using System.Collections;
using Avalonia;
using Avalonia.Controls;

namespace TodoApp.Views.Components;

public partial class ValidationErrorsView : UserControl
{
    public static readonly StyledProperty<IEnumerable> ErrorsProperty =
        AvaloniaProperty.Register<ValidationErrorsView, IEnumerable>(nameof(Errors));

    public ValidationErrorsView()
    {
        InitializeComponent();
    }

    public IEnumerable Errors
    {
        get => GetValue(ErrorsProperty);
        set => SetValue(ErrorsProperty, value);
    }
}
