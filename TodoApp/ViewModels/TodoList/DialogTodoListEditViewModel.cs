using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.Utils;
using TodoListModel = TodoApp.Api.TodoList;


namespace TodoApp.ViewModels.TodoList;

public sealed partial class DialogTodoListEditViewModel : ViewModelBase, IDialogAware

{
    private readonly IClient _api;


    [ObservableProperty] private string _description = string.Empty;

    private int _id;


    [ObservableProperty] private string _name = string.Empty;


    [ObservableProperty] private string _validationErrors = string.Empty;


    public DialogTodoListEditViewModel(
        IClient api,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)

    {
        _api = api;
        RequestClose = default!;

        var ui = AvaloniaScheduler.Instance;

        SaveCommand = commandFactory.CreateFromTask(SaveAsync, nameof(DialogTodoListEditViewModel),
            nameof(SaveCommand), ui);

        CancelCommand = commandFactory.Create(Cancel, nameof(DialogTodoListEditViewModel),
            nameof(CancelCommand), ui);
    }


    public RxCommand<Unit, Unit> SaveCommand { get; }


    public RxCommand<Unit, Unit> CancelCommand { get; }


    public DialogCloseListener RequestClose { get; }


    public bool CanCloseDialog()
    {
        return true;
    }


    public void OnDialogClosed()

    {
    }


    public void OnDialogOpened(IDialogParameters parameters)

    {
        var item = parameters.GetValue<TodoListModel>("item");

        _id = item.Id;

        Name = item.Name;

        Description = item.Description ?? string.Empty;

        ValidationErrors = string.Empty;
    }


    private async Task SaveAsync()

    {
        ValidationErrors = string.Empty;


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
