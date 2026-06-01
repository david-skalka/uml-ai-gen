using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.ViewModels;
using TodoApp.ViewModels.Alarm;
using TodoApp.ViewModels.TodoList;

namespace TodoApp.ViewModels.Shell;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel(
        TodoListPageViewModel todoListPage,
        AlarmPageViewModel alarmPage,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        TodoListPage = todoListPage;
        AlarmPage = alarmPage;
        CurrentPage = todoListPage;

        var ui = AvaloniaScheduler.Instance;

        NavigateTodoListCommand = commandFactory.CreateFromTask(NavigateTodoListAsync, nameof(MainWindowViewModel),
            nameof(NavigateTodoListCommand), ui);
        NavigateAlarmCommand = commandFactory.CreateFromTask(NavigateAlarmAsync, nameof(MainWindowViewModel),
            nameof(NavigateAlarmCommand), ui);
    }

    public TodoListPageViewModel TodoListPage { get; }

    public AlarmPageViewModel AlarmPage { get; }

    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private bool _isTodoListNavActive = true;

    [ObservableProperty]
    private bool _isAlarmNavActive;

    public RxCommand<Unit, Unit> NavigateTodoListCommand { get; }

    public RxCommand<Unit, Unit> NavigateAlarmCommand { get; }

    public async Task InitializeAsync()
    {
        await NavigateTodoListAsync();
    }

    private async Task NavigateTodoListAsync()
    {
        IsTodoListNavActive = true;
        IsAlarmNavActive = false;
        CurrentPage = TodoListPage;
        await TodoListPage.InitializeAsync();
    }

    private async Task NavigateAlarmAsync()
    {
        IsTodoListNavActive = false;
        IsAlarmNavActive = true;
        CurrentPage = AlarmPage;
        await AlarmPage.InitializeAsync();
    }
}
