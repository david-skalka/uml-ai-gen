using Avalonia.Controls;
using Prism.Dialogs;

namespace TodoApp.Views.Dialogs;

public partial class MyDialogWindow : Window, IDialogWindow
{
    public MyDialogWindow()
    {
        InitializeComponent();
    }

    public IDialogResult Result { get; set; } = new DialogResult(ButtonResult.None);
}
