using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.ViewModels.Dialogs;
using TodoApp.Views.Alarm;
using AlarmModel = TodoApp.Api.Alarm;

namespace TodoApp.ViewModels.Alarm;

public partial class AlarmPageViewModel : ViewModelBase
{
    private readonly IClient _api;
    private readonly IAppDialogService _dialogService;

    [ObservableProperty] private AlarmModel? _selectedItem;

    public AlarmPageViewModel(
        IClient api,
        IAppDialogService dialogService,
        IErrorHandlerService errorHandlerService,
        ICommandFactory commandFactory)
        : base(errorHandlerService)
    {
        _api = api;
        _dialogService = dialogService;

        var ui = AvaloniaScheduler.Instance;

        NewCommand = commandFactory.CreateFromTask(NewAsync, nameof(AlarmPageViewModel), nameof(NewCommand), ui);
        EditCommand = commandFactory.CreateFromTask(EditAsync, nameof(AlarmPageViewModel), nameof(EditCommand),
            Observable.Return(true), ui);
        DeleteCommand = commandFactory.CreateFromTask(DeleteAsync, nameof(AlarmPageViewModel), nameof(DeleteCommand),
            Observable.Return(true), ui);
    }

    public ObservableCollection<AlarmModel> Items { get; } = [];

    public RxCommand<Unit, Unit> NewCommand { get; }

    public RxCommand<Unit, Unit> EditCommand { get; }

    public RxCommand<Unit, Unit> DeleteCommand { get; }

    public async Task InitializeAsync()
    {
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        var items = await _api.AlarmsGetAllAsync();
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);
    }

    private async Task NewAsync()
    {
        await _dialogService.ShowAsync<DialogAlarmEditView, DialogAlarmEditViewModel>(
            new DialogParameters
            {
                { "item", new AlarmModel { Id = 0, Title = string.Empty, Time = DateTimeOffset.Now } }
            });
        await LoadAsync();
    }

    private async Task EditAsync()
    {
        await _dialogService.ShowAsync<DialogAlarmEditView, DialogAlarmEditViewModel>(
            new DialogParameters { { "item", SelectedItem! } });

        await LoadAsync();
    }


    private async Task DeleteAsync()
    {
        var item = SelectedItem;
        var confirm = await _dialogService.ShowNotificationAsync(
            "Delete Alarm",
            $"Delete \"{item!.Title}\"?",
            [
                new DialogButton("Cancel", ButtonResult.Cancel),
                new DialogButton("Delete", ButtonResult.OK)
            ]);

        if (confirm.Result != ButtonResult.OK)
            return;

        await _api.AlarmsDeleteAsync(item.Id);

        await LoadAsync();
    }
}
