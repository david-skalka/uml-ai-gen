using System.Collections.ObjectModel;
using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Infrastructure;

namespace TodoApp.ViewModels.Dialogs;

public class DialogButton(string label, ButtonResult result)
{
    public string Label { get; set; } = label;

    public ButtonResult Result { get; set; } = result;
}

public sealed partial class DialogNotificationViewModel : ObservableObject, IDialogAware
{
    [ObservableProperty] private ObservableCollection<DialogButton> _buttons = [];

    [ObservableProperty] private string _message = string.Empty;

    [ObservableProperty] private string _title = string.Empty;

    public DialogNotificationViewModel(ICommandFactory commandFactory)
    {
        RequestClose = default!;
        ClickButtonCommand = commandFactory.Create<ButtonResult>(ClickButton, nameof(DialogNotificationViewModel),
            nameof(ClickButtonCommand), AvaloniaScheduler.Instance);
    }

    public RxCommand<ButtonResult, Unit> ClickButtonCommand { get; }

    public DialogCloseListener RequestClose { get; }

    public bool CanCloseDialog()
    {
        return true;
    }

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title");
        Message = parameters.GetValue<string>("message");
        Buttons = new ObservableCollection<DialogButton>(parameters.GetValue<IEnumerable<DialogButton>>("buttons"));
    }

    private void ClickButton(ButtonResult result)
    {
        RequestClose.Invoke(new DialogResult(result));
    }
}
