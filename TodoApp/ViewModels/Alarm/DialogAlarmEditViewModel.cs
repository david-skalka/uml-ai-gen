using System.Reactive;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.Utils;
using AlarmModel = TodoApp.Api.Alarm;

namespace TodoApp.ViewModels.Alarm;

public sealed partial class DialogAlarmEditViewModel : DialogViewModelBase
{
    private readonly IClient _api;

    [ObservableProperty] private string _alarmTitle = string.Empty;

    private int _id;

    [ObservableProperty] private DateTimeOffset _time;

    [ObservableProperty] private string _validationErrors = string.Empty;

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

    public RxCommand<Unit, Unit> SaveCommand { get; }

    public RxCommand<Unit, Unit> CancelCommand { get; }

    public override void OnDialogOpened(IDialogParameters parameters)
    {
        var item = parameters.GetValue<AlarmModel>("item");
        _id = item.Id;
        AlarmTitle = item.Title;
        Time = item.Time!.Value;
        ValidationErrors = string.Empty;
    }

    private async Task SaveAsync()
    {
        ValidationErrors = string.Empty;

        try
        {
            var result = new DialogResult(ButtonResult.OK);
            var trimmedTitle = AlarmTitle.Trim();

            if (_id == 0)
            {
                var created = await _api.AlarmsCreateAsync(new AlarmModel { Title = trimmedTitle, Time = Time });
                result.Parameters.Add("item", created);
            }
            else
            {
                var updated = await _api.AlarmsUpdateAsync(new AlarmModel
                {
                    Id = _id, Title = trimmedTitle, Time = Time
                });
                result.Parameters.Add("item", updated);
            }

            CloseDialog(result);
        }
        catch (ApiException ex) when (ex.StatusCode == 400)
        {
            ValidationErrors = ex.FormatValidationErrors();
        }
    }

    private void Cancel()
    {
        CloseDialog(new DialogResult(ButtonResult.Cancel));
    }
}
