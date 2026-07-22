using System.Reactive;
using TodoApp.Infrastructure;
using TodoApp.Services;

namespace TodoApp.ViewModels.Shell;

public class MainWindowViewModel : ViewModelBase
{
    private readonly IRegionManager _regionManager;

    public MainWindowViewModel(
        IRegionManager regionManager,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _regionManager = regionManager;

        var ui = AvaloniaScheduler.Instance;

        NavigateCommand = commandFactory.Create<string>(Navigate, nameof(MainWindowViewModel),
            nameof(NavigateCommand), ui);
    }

    public RxCommand<string, Unit> NavigateCommand { get; }

    private void Navigate(string viewName) =>
        _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
}
