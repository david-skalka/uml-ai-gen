using System.Reactive;
using ShadUI;
using TodoApp.Infrastructure;
using TodoApp.Services;

namespace TodoApp.ViewModels.Shell;

public class MainWindowViewModel : ViewModelBase
{
    private readonly DialogManagerHolder _dialogManagerHolder;
    private readonly IRegionManager _regionManager;

    public MainWindowViewModel(
        IRegionManager regionManager,
        DialogManagerHolder dialogManagerHolder,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _regionManager = regionManager;
        _dialogManagerHolder = dialogManagerHolder;

        var ui = AvaloniaScheduler.Instance;

        NavigateCommand = commandFactory.Create<string>(Navigate, nameof(MainWindowViewModel),
            nameof(NavigateCommand), ui);
    }

    public DialogManager DialogManager => _dialogManagerHolder.Manager;

    public RxCommand<string, Unit> NavigateCommand { get; }

    private void Navigate(string viewName) =>
        _regionManager.RequestNavigate(RegionNames.ContentRegion, viewName);
}
