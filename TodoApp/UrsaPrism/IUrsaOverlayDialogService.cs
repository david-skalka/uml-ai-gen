using Ursa.Controls;

namespace TodoApp.UrsaPrism;

public interface IUrsaOverlayDialogService
{
    Task<TResult?> ShowCustomAsync<TResult>(
        string viewName,
        object? vm,
        string? hostId = null,
        OverlayDialogOptions? options = null);
}
