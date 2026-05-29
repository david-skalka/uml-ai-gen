using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.ViewModels;
using TodoApp.ViewModels.Dialogs;
using Prism.Dialogs;
using TodoListModel = TodoApp.Api.TodoList;

namespace TodoApp.ViewModels.TodoList;

public partial class TodoListPageViewModel : ViewModelBase, IDisposable
{
    private readonly IClient _api;
    private readonly ICommandFactory _commandFactory;
    private readonly IDialogService _dialogService;

    public TodoListPageViewModel(
        IClient api,
        IDialogService dialogService,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _api = api;
        _dialogService = dialogService;
        _commandFactory = commandFactory;

        var ui = AvaloniaScheduler.Instance;

        LoadCommand = _commandFactory.CreateFromTask(LoadAsync, nameof(TodoListPageViewModel), nameof(LoadCommand), ui);
        NewCommand = _commandFactory.CreateFromTask(NewAsync, nameof(TodoListPageViewModel), nameof(NewCommand), ui);
        EditCommand = _commandFactory.CreateFromTask(EditAsync, nameof(TodoListPageViewModel), nameof(EditCommand),
            Observable.Return(true), ui);
        DeleteCommand = _commandFactory.CreateFromTask(DeleteAsync, nameof(TodoListPageViewModel), nameof(DeleteCommand),
            Observable.Return(true), ui);
        GroupByNameCommand = _commandFactory.CreateFromTask(GroupByNameAsync, nameof(TodoListPageViewModel),
            nameof(GroupByNameCommand), ui);
    }

    public ObservableCollection<TodoListModel> Items { get; } = [];

    public ObservableCollection<GroupByNameOutput> GroupByNameResults { get; } = [];

    [ObservableProperty]
    private TodoListModel? _selectedItem;

    [ObservableProperty]
    private bool _includeArchived;

    public RxCommand<Unit, Unit> LoadCommand { get; }

    public RxCommand<Unit, Unit> NewCommand { get; }

    public RxCommand<Unit, Unit> EditCommand { get; }

    public RxCommand<Unit, Unit> DeleteCommand { get; }

    public RxCommand<Unit, Unit> GroupByNameCommand { get; }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }

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

        SelectedItem = Items.FirstOrDefault();
    }

    private async Task NewAsync()
    {
        var result = await _dialogService.ShowDialogAsync("todo-list-edit", new DialogParameters
        {
            { "isEdit", false },
            { "id", 0 },
            { "name", string.Empty },
            { "description", string.Empty }
        });

        if (result.Result != ButtonResult.OK)
            return;

        var created = result.Parameters.GetValue<TodoListModel>("item")!;
        Items.Insert(0, created);
        SelectedItem = created;
    }

    private async Task EditAsync()
    {
        if (SelectedItem is null)
            return;

        var item = SelectedItem;
        var result = await _dialogService.ShowDialogAsync("todo-list-edit", new DialogParameters
        {
            { "isEdit", true },
            { "id", item.Id },
            { "name", item.Name ?? string.Empty },
            { "description", item.Description ?? string.Empty }
        });

        if (result.Result != ButtonResult.OK)
            return;

        var updated = result.Parameters.GetValue<TodoListModel>("item")!;
        item.Name = updated.Name;
        item.Description = updated.Description;
        ReselectCurrentItem();
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null)
            return;

        var item = SelectedItem;
        var confirm = await _dialogService.ShowNotificationAsync(
            "Delete Todo List",
            $"Delete \"{item.Name}\"?",
            [
                new DialogButton("Cancel", ButtonResult.Cancel),
                new DialogButton("Delete", ButtonResult.OK)
            ]);

        if (confirm.Result != ButtonResult.OK)
            return;

        await _api.TodoListsDeleteAsync(item.Id);
        Items.Remove(item);
        SelectedItem = Items.FirstOrDefault();
    }

    private async Task GroupByNameAsync()
    {
        var results = await _api.TodoListsGroupByNameAsync(new GroupByNameInput
        {
            IncludeArchived = IncludeArchived
        });

        GroupByNameResults.Clear();
        foreach (var result in results)
            GroupByNameResults.Add(result);
    }

    private void ReselectCurrentItem()
    {
        var current = SelectedItem;
        SelectedItem = null;
        SelectedItem = current;
    }
}
