using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Prism.Dialogs;
using TodoApp.Views.Shell;

namespace TodoApp.Views.Dialogs;

public partial class MyDialogWindow : Window, IDialogWindow
{
    public MyDialogWindow(IActiveDialogTracker tracker)
    {
        InitializeComponent();
        tracker.ActiveDialog = this;
    }

    public IDialogResult Result { get; set; } = new DialogResult(ButtonResult.None);
}
