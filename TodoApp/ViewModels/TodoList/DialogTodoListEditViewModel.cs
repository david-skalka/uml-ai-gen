using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.Utils;
using TodoApp.ViewModels;
using Prism.Dialogs;

namespace TodoApp.ViewModels.TodoList;

public sealed partial class DialogTodoListEditViewModel : ViewModelBase, IDialogAware
{
    private readonly IClient _api;
    private bool _isEdit;
    private Guid _id;

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

    [ObservableProperty]
    private string _title = "Todo List";

    [ObservableProperty]
    private string _name = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _validationErrors = string.Empty;

    public RxCommand<Unit, Unit> SaveCommand { get; }

    public RxCommand<Unit, Unit> CancelCommand { get; }

    public DialogCloseListener RequestClose { get; set; }

    public bool CanCloseDialog() => true;

    public void OnDialogClosed()
    {
    }

    public void OnDialogOpened(IDialogParameters parameters)
    {
        _isEdit = parameters.GetValue<bool>("isEdit");
        _id = parameters.GetValue<Guid>("id");
        Title = _isEdit ? "Edit Todo List" : "New Todo List";
        Name = parameters.GetValue<string>("name") ?? string.Empty;
        Description = parameters.GetValue<string>("description") ?? string.Empty;
        ValidationErrors = string.Empty;
    }

    private async Task SaveAsync()
    {
        ValidationErrors = string.Empty;

        try
        {
            var result = new DialogResult(ButtonResult.OK);

            if (_isEdit)
            {
                var updated = await _api.TodoListsUpdateAsync(_id, new UpdateTodoListRequest
                {
                    Name = Name.Trim(),
                    Description = Description.Trim()
                });
                result.Parameters.Add("item", updated);
            }
            else
            {
                var created = await _api.TodoListsCreateAsync(new CreateTodoListRequest
                {
                    Name = Name.Trim(),
                    Description = Description.Trim()
                });
                result.Parameters.Add("item", created);
            }

            RequestClose.Invoke(result);
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            ValidationErrors = ex.FormatValidationErrors();
        }
    }

    private void Cancel()
    {
        RequestClose.Invoke(new DialogResult(ButtonResult.Cancel));
    }
}
