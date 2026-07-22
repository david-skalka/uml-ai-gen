using System.Collections.ObjectModel;
using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;

namespace TodoApp.ViewModels.TodoList;

public sealed partial class DialogGroupByNameViewModel : DialogViewModelBase
{
    private readonly IClient _api;

    [ObservableProperty] private bool _includeArchived;

    public DialogGroupByNameViewModel(
        IClient api,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _api = api;

        var ui = AvaloniaScheduler.Instance;

        RunCommand = commandFactory.CreateFromTask(RunAsync, nameof(DialogGroupByNameViewModel),
            nameof(RunCommand), ui);
        CancelCommand = commandFactory.Create(Cancel, nameof(DialogGroupByNameViewModel),
            nameof(CancelCommand), ui);
    }

    public RxCommand<Unit, Unit> RunCommand { get; }

    public RxCommand<Unit, Unit> CancelCommand { get; }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        IncludeArchived = parameters.GetValue<bool>("includeArchived");
    }

    private async Task RunAsync()
    {
        var results = await _api.TodoListsGroupByNameAsync(new GroupByNameInput
        {
            IncludeArchived = IncludeArchived
        });

        var result = new DialogResult(ButtonResult.OK);
        result.Parameters.Add("results", new ObservableCollection<GroupByNameOutput>(results));
        CloseDialog(result);
    }

    private void Cancel()
    {
        CloseDialog(new DialogResult(ButtonResult.Cancel));
    }
}
