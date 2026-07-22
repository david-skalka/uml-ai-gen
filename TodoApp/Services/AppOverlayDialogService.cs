using Avalonia;
using Avalonia.Controls;
using Ursa.Controls;
using TodoApp.UrsaPrism;

namespace TodoApp.Services;

public sealed class AppOverlayDialogService(
    IUrsaOverlayDialogService overlayDialogs,
    IContainerExtension container) : IAppDialogService
{
    public async Task<IDialogResult> ShowAsync<TView, TViewModel>(IDialogParameters parameters)
        where TView : Control
        where TViewModel : class, IDialogAware
    {
        var dialogAware = container.Resolve<TViewModel>();
        dialogAware.OnDialogOpened(parameters);

        var result = await overlayDialogs.ShowCustomAsync<IDialogResult>(
            typeof(TView).Name,
            dialogAware,
            hostId: null,
            new OverlayDialogOptions
            {
                CanLightDismiss = false,
                IsCloseButtonVisible = true,
                TopLevelHashCode = ((App)Application.Current!).MainWindow.GetHashCode()
            });

        if (dialogAware.CanCloseDialog())
            dialogAware.OnDialogClosed();

        return result ?? new Prism.Dialogs.DialogResult(ButtonResult.Cancel);
    }
}
