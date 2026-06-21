using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.ViewModels.Alarm;
using TodoApp.ViewModels.TodoList;

namespace TodoApp.ViewModels.Shell;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly AlarmPageViewModel _alarmPage;
    private readonly TodoListPageViewModel _todoListPage;

    [ObservableProperty] private ViewModelBase _currentPage;
    [ObservableProperty] private string _currentRoute = "todo-list";

    public MainWindowViewModel(
        TodoListPageViewModel todoListPage,
        AlarmPageViewModel alarmPage,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _todoListPage = todoListPage;
        _alarmPage = alarmPage;
        CurrentPage = todoListPage;

        var ui = AvaloniaScheduler.Instance;

        NavigateTodoListCommand = commandFactory.CreateFromTask(NavigateTodoListAsync, nameof(MainWindowViewModel),
            nameof(NavigateTodoListCommand), ui);
        NavigateAlarmCommand = commandFactory.CreateFromTask(NavigateAlarmAsync, nameof(MainWindowViewModel),
            nameof(NavigateAlarmCommand), ui);
    }

    public RxCommand<Unit, Unit> NavigateTodoListCommand { get; }

    public RxCommand<Unit, Unit> NavigateAlarmCommand { get; }

    public async Task InitializeAsync()
    {
        await NavigateTodoListAsync();
    }

    private async Task NavigateTodoListAsync()
    {
        CurrentRoute = "todo-list";
        CurrentPage = _todoListPage;
        await _todoListPage.InitializeAsync();
    }

    private async Task NavigateAlarmAsync()
    {
        CurrentRoute = "alarm";
        CurrentPage = _alarmPage;
        await _alarmPage.InitializeAsync();
    }
}
