using Irihi.Avalonia.Shared.Contracts;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public abstract class DialogViewModelBase(IErrorHandlerService errorHandlerService)
    : ViewModelBase(errorHandlerService), IDialogAware, IDialogContext
{
    private EventHandler<object?>? _dialogRequestClose;

    public DialogCloseListener RequestClose { get; } = default!;

    event EventHandler<object?>? IDialogContext.RequestClose
    {
        add => _dialogRequestClose += value;
        remove => _dialogRequestClose -= value;
    }

    public void Close() => CloseDialog(new DialogResult(ButtonResult.Cancel));

    public virtual bool CanCloseDialog() => true;

    public virtual void OnDialogClosed()
    {
    }

    public abstract void OnDialogOpened(IDialogParameters parameters);

    protected void CloseDialog(IDialogResult result) => _dialogRequestClose?.Invoke(this, result);
}
