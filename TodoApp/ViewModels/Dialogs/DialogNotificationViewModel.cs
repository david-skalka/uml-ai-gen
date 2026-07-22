using System.Collections.ObjectModel;
using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using Irihi.Avalonia.Shared.Contracts;
using TodoApp.Infrastructure;

namespace TodoApp.ViewModels.Dialogs;

public class DialogButton(string label, ButtonResult result)
{
    public string Label { get; set; } = label;

    public ButtonResult Result { get; set; } = result;
}

public sealed partial class DialogNotificationViewModel : ObservableObject, IDialogAware, IDialogContext
{
    private EventHandler<object?>? _dialogRequestClose;

    [ObservableProperty] private ObservableCollection<DialogButton> _buttons = [];

    [ObservableProperty] private IReadOnlyList<string> _errors = [];

    [ObservableProperty] private bool _hasErrors;

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

    event EventHandler<object?>? IDialogContext.RequestClose
    {
        add => _dialogRequestClose += value;
        remove => _dialogRequestClose -= value;
    }

    public void Close() => CloseDialog(new DialogResult(ButtonResult.Cancel));

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

        Message = string.Empty;
        if (parameters.ContainsKey("message"))
            Message = parameters.GetValue<string>("message");

        Errors = [];
        if (parameters.ContainsKey("errors"))
            Errors = parameters.GetValue<IEnumerable<string>>("errors").ToList();

        HasErrors = Errors.Count > 0;
        Buttons = new ObservableCollection<DialogButton>(parameters.GetValue<IEnumerable<DialogButton>>("buttons"));
    }

    private void ClickButton(ButtonResult result)
    {
        CloseDialog(new DialogResult(result));
    }

    private void CloseDialog(IDialogResult result) => _dialogRequestClose?.Invoke(this, result);
}
