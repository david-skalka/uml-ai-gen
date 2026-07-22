using Avalonia;
using Avalonia.Controls;
using Irihi.Avalonia.Shared.Contracts;
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

        var result = await ShowCustomAsync((dynamic)dialogAware, typeof(TView).Name);

        if (dialogAware.CanCloseDialog())
            dialogAware.OnDialogClosed();

        return result ?? new Prism.Dialogs.DialogResult(ButtonResult.Cancel);
    }

    private Task<IDialogResult?> ShowCustomAsync<TViewModel>(TViewModel viewModel, string viewName)
        where TViewModel : class, IDialogAware, IDialogContext
    {
        return overlayDialogs.ShowCustomAsync<IDialogResult>(
            viewName,
            viewModel,
            hostId: null,
            new OverlayDialogOptions
            {
                CanLightDismiss = false,
                IsCloseButtonVisible = true,
                TopLevelHashCode = ResolveTopLevelHashCode()
            });
    }

    private static int ResolveTopLevelHashCode()
    {
        var app = (App)Application.Current!;
        return app.MainWindow.GetHashCode();
    }
}
