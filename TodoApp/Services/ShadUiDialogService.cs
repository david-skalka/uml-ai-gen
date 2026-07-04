using Avalonia.Controls;
using ShadUI;

namespace TodoApp.Services;

public sealed class ShadUiDialogService(IContainerExtension container, DialogManagerHolder dialogManagerHolder)
    : IAppDialogService
{
    public Task<IDialogResult> ShowAsync<TView, TViewModel>(IDialogParameters parameters)
        where TView : Control
        where TViewModel : class, IDialogAware
    {
        var completion = new TaskCompletionSource<IDialogResult>();
        var dialogManager = dialogManagerHolder.Manager;
        var dialogAware = container.Resolve<TViewModel>();

        DialogUtilities.InitializeListener(dialogAware, result =>
        {
            if (!dialogAware.CanCloseDialog())
                return Task.CompletedTask;

            dialogAware.OnDialogClosed();
            dialogManager.AllowDismissal();
            dialogManager.Close(dialogAware, new CloseDialogOptions
            {
                Success = result.Result == ButtonResult.OK || result.Result == ButtonResult.Yes
            });
            completion.TrySetResult(result);
            return Task.CompletedTask;
        });

        dialogAware.OnDialogOpened(parameters);

        dialogManager.PreventDismissal();
        dialogManager.CreateDialog(dialogAware).Show();

        return completion.Task;
    }
}
