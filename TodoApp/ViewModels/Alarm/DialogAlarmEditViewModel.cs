using System.Globalization;
using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.Utils;
using TodoApp.ViewModels;
using Prism.Dialogs;

namespace TodoApp.ViewModels.Alarm;

public sealed partial class DialogAlarmEditViewModel : ViewModelBase, IDialogAware
{
    private readonly IClient _api;
    private bool _isEdit;
    private Guid _id;

    public DialogAlarmEditViewModel(
        IClient api,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _api = api;
        var ui = AvaloniaScheduler.Instance;
        SaveCommand = commandFactory.CreateFromTask(SaveAsync, nameof(DialogAlarmEditViewModel),
            nameof(SaveCommand), ui);
        CancelCommand = commandFactory.Create(Cancel, nameof(DialogAlarmEditViewModel),
            nameof(CancelCommand), ui);
    }

    [ObservableProperty]
    private string _title = "Alarm";

    [ObservableProperty]
    private string _alarmTitle = string.Empty;

    [ObservableProperty]
    private string _time = string.Empty;

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
        Title = _isEdit ? "Edit Alarm" : "New Alarm";
        AlarmTitle = parameters.GetValue<string>("title") ?? string.Empty;
        Time = parameters.GetValue<string>("time") ?? string.Empty;
        ValidationErrors = string.Empty;
    }

    private async Task SaveAsync()
    {
        ValidationErrors = string.Empty;

        if (!DateTimeOffset.TryParse(Time, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedTime))
        {
            ValidationErrors = "Time must be yyyy-MM-dd HH:mm.";
            return;
        }

        try
        {
            var result = new DialogResult(ButtonResult.OK);

            if (_isEdit)
            {
                var updated = await _api.AlarmsUpdateAsync(_id, new UpdateAlarmRequest
                {
                    Title = AlarmTitle.Trim(),
                    Time = parsedTime
                });
                result.Parameters.Add("item", updated);
            }
            else
            {
                var created = await _api.AlarmsCreateAsync(new CreateAlarmRequest
                {
                    Title = AlarmTitle.Trim(),
                    Time = parsedTime
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
