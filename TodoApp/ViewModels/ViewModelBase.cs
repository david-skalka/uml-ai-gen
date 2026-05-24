using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Services;

namespace TodoApp.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    protected ViewModelBase(IErrorHandlerService errorHandlerService)
    {
        _ = errorHandlerService;
    }
}
