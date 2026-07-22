using Ursa.Controls;

namespace TodoApp.UrsaPrism;

internal sealed class UrsaOverlayDialogService : IUrsaOverlayDialogService
{
    public Task<TResult?> ShowCustomAsync<TResult>(
        string viewName,
        object? vm,
        string? hostId = null,
        OverlayDialogOptions? options = null)
    {
        var view = UrsaDialogServiceExtension.CreateView(viewName);
        return OverlayDialog.ShowCustomAsync<TResult>(view, vm, hostId, options);
    }
}
