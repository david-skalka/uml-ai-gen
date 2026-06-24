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
        ShowAsyncInternal<TViewModel>(parameters, completion);
        return completion.Task;
    }

    private void ShowAsyncInternal<TViewModel>(
        IDialogParameters parameters,
        TaskCompletionSource<IDialogResult> completion)
        where TViewModel : class, IDialogAware
    {
        try
        {
            var dialogManager = dialogManagerHolder.Manager;
            var dialogAware = container.Resolve<TViewModel>();

            DialogUtilities.InitializeListener(dialogAware, result =>
            {
                if (!dialogAware.CanCloseDialog())
                    return Task.CompletedTask;

                dialogAware.OnDialogClosed();
                dialogManager.AllowDismissal();
                CloseDialog(dialogManager, dialogAware, new CloseDialogOptions
                {
                    Success = result.Result == ButtonResult.OK || result.Result == ButtonResult.Yes
                });
                completion.TrySetResult(result);
                return Task.CompletedTask;
            });

            dialogAware.OnDialogOpened(parameters);

            dialogManager.PreventDismissal();
            ShowDialog(dialogManager, dialogAware);
        }
        catch (Exception ex)
        {
            completion.TrySetException(ex);
        }
    }

    private static void ShowDialog(DialogManager dialogManager, object context)
    {
        ShowDialogCore(dialogManager, (dynamic)context);
    }

    private static void CloseDialog(DialogManager dialogManager, object context, CloseDialogOptions options)
    {
        CloseDialogCore(dialogManager, (dynamic)context, options);
    }

    private static void ShowDialogCore<TContext>(DialogManager dialogManager, TContext context)
        where TContext : class
    {
        dialogManager.CreateDialog(context).Show();
    }

    private static void CloseDialogCore<TContext>(
        DialogManager dialogManager,
        TContext context,
        CloseDialogOptions options)
        where TContext : class
    {
        dialogManager.Close(context, options);
    }
}
