using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.Utils;
using TodoListModel = TodoApp.Api.TodoList;

namespace TodoApp.ViewModels.TodoList;

public sealed partial class DialogTodoListEditViewModel : DialogViewModelBase
{
    private readonly IClient _api;

    [ObservableProperty] private string _description = string.Empty;

    private int _id;

    [ObservableProperty] private string _name = string.Empty;

    [ObservableProperty] private IReadOnlyList<string> _validationErrors = [];

    public DialogTodoListEditViewModel(
        IClient api,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _api = api;

        var ui = AvaloniaScheduler.Instance;

        SaveCommand = commandFactory.CreateFromTask(SaveAsync, nameof(DialogTodoListEditViewModel),
            nameof(SaveCommand), ui);

        CancelCommand = commandFactory.Create(Cancel, nameof(DialogTodoListEditViewModel),
            nameof(CancelCommand), ui);
    }

    public RxCommand<Unit, Unit> SaveCommand { get; }

    public RxCommand<Unit, Unit> CancelCommand { get; }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        var item = parameters.GetValue<TodoListModel>("item");

        _id = item.Id;

        Name = item.Name;

        Description = item.Description ?? string.Empty;

        ValidationErrors = [];
    }

    private async Task SaveAsync()
    {
        ValidationErrors = [];

        try
        {
            var result = new DialogResult(ButtonResult.OK);

            var trimmedName = Name.Trim();

            var trimmedDescription = Description.Trim();

            if (_id == 0)
            {
                var created = await _api.TodoListsCreateAsync(new TodoListModel
                {
                    Name = trimmedName, Description = trimmedDescription
                });

                result.Parameters.Add("item", created);
            }
            else
            {
                var updated = await _api.TodoListsUpdateAsync(new TodoListModel
                {
                    Id = _id, Name = trimmedName, Description = trimmedDescription
                });

                result.Parameters.Add("item", updated);
            }

            CloseDialog(result);
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            ValidationErrors = ex.GetValidationErrors();
        }
    }

    private void Cancel()
    {
        CloseDialog(new DialogResult(ButtonResult.Cancel));
    }
}
