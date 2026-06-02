using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.ViewModels.Dialogs;
using TodoListModel = TodoApp.Api.TodoList;

namespace TodoApp.ViewModels.TodoList;

public partial class TodoListPageViewModel : ViewModelBase
{
    private readonly IClient _api;
    private readonly IDialogService _dialogService;

    [ObservableProperty] private bool _includeArchived;

    [ObservableProperty] private TodoListModel? _selectedItem;

    public TodoListPageViewModel(
        IClient api,
        IDialogService dialogService,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _api = api;
        _dialogService = dialogService;

        var ui = AvaloniaScheduler.Instance;

        NewCommand = commandFactory.CreateFromTask(NewAsync, nameof(TodoListPageViewModel), nameof(NewCommand), ui);
        EditCommand = commandFactory.CreateFromTask(EditAsync, nameof(TodoListPageViewModel), nameof(EditCommand),
            Observable.Return(true), ui);
        DeleteCommand = commandFactory.CreateFromTask(DeleteAsync, nameof(TodoListPageViewModel),
            nameof(DeleteCommand),
            Observable.Return(true), ui);
        GroupByNameCommand = commandFactory.CreateFromTask(GroupByNameAsync, nameof(TodoListPageViewModel),
            nameof(GroupByNameCommand), ui);
    }

    public ObservableCollection<TodoListModel> Items { get; } = [];

    public ObservableCollection<GroupByNameOutput> GroupByNameResults { get; } = [];

    public RxCommand<Unit, Unit> NewCommand { get; }

    public RxCommand<Unit, Unit> EditCommand { get; }

    public RxCommand<Unit, Unit> DeleteCommand { get; }

    public RxCommand<Unit, Unit> GroupByNameCommand { get; }

    public async Task InitializeAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var items = await _api.TodoListsGetAllAsync();
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);
    }

    private async Task NewAsync()
    {
        await _dialogService.ShowDialogAsync("todo-list-edit",
            new DialogParameters
            {
                { "item", new TodoListModel { Id = 0, Name = string.Empty, Description = string.Empty } }
            });
        await LoadAsync();
    }

    private async Task EditAsync()
    {
        await _dialogService.ShowDialogAsync("todo-list-edit", new DialogParameters { { "item", SelectedItem! } });
        await LoadAsync();
    }

    private async Task DeleteAsync()
    {
        var item = SelectedItem;
        var confirm = await _dialogService.ShowNotificationAsync(
            "Delete Todo List",
            $"Delete \"{item!.Name}\"?",
            [
                new DialogButton("Cancel", ButtonResult.Cancel),
                new DialogButton("Delete", ButtonResult.OK)
            ]);

        if (confirm.Result != ButtonResult.OK)
            return;

        await _api.TodoListsDeleteAsync(item.Id);
        await LoadAsync();
    }

    private async Task GroupByNameAsync()
    {
        var results = await _api.TodoListsGroupByNameAsync(new GroupByNameInput { IncludeArchived = IncludeArchived });

        GroupByNameResults.Clear();
        foreach (var result in results)
            GroupByNameResults.Add(result);
    }
}
