using System.Collections.ObjectModel;
using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Infrastructure;
using Prism.Dialogs;

namespace TodoApp.ViewModels.Dialogs;

public class DialogButton(string label, ButtonResult result)
{
    public string Label { get; set; } = label;

    public ButtonResult Result { get; set; } = result;
}

public sealed partial class DialogNotificationViewModel : ObservableObject, IDialogAware
{
    public DialogNotificationViewModel(ICommandFactory commandFactory)
    {
        ClickButtonCommand = commandFactory.Create<ButtonResult>(ClickButton, nameof(DialogNotificationViewModel),
            nameof(ClickButtonCommand), AvaloniaScheduler.Instance);
    }

    [ObservableProperty]
    private string _message = string.Empty;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private ObservableCollection<string> _errors = [];

    [ObservableProperty]
    private bool _hasErrors;

    [ObservableProperty]
    private ObservableCollection<DialogButton> _buttons = [];

    public RxCommand<ButtonResult, Unit> ClickButtonCommand { get; }

    public DialogCloseListener RequestClose { get; set; }

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title") ?? string.Empty;
        Message = parameters.GetValue<string>("message") ?? string.Empty;
        Errors = new ObservableCollection<string>(parameters.GetValue<IEnumerable<string>>("errors") ?? []);
        HasErrors = Errors.Count > 0;
        Buttons = new ObservableCollection<DialogButton>(
            parameters.GetValue<IEnumerable<DialogButton>>("buttons") ?? []);
    }

    private void ClickButton(ButtonResult result)
    {
        RequestClose.Invoke(new DialogResult(result));
    }
}
