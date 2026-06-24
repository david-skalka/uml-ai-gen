using Avalonia.Controls;

namespace TodoApp.Services;

public interface IAppDialogService
{
    Task<IDialogResult> ShowAsync<TView, TViewModel>(IDialogParameters parameters)
        where TView : Control
        where TViewModel : class, IDialogAware;
}
