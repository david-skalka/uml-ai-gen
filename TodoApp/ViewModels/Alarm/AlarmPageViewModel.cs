using System.Collections.ObjectModel;
using System.Globalization;
using System.Reactive;
using System.Reactive.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.ViewModels;
using TodoApp.ViewModels.Dialogs;
using Prism.Dialogs;
using AlarmModel = TodoApp.Api.Alarm;

namespace TodoApp.ViewModels.Alarm;

public partial class AlarmPageViewModel : ViewModelBase, IDisposable
{
    private readonly IClient _api;
    private readonly ICommandFactory _commandFactory;
    private readonly IDialogService _dialogService;

    public AlarmPageViewModel(
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

        LoadCommand = _commandFactory.CreateFromTask(LoadAsync, nameof(AlarmPageViewModel), nameof(LoadCommand), ui);
        NewCommand = _commandFactory.CreateFromTask(NewAsync, nameof(AlarmPageViewModel), nameof(NewCommand), ui);
        EditCommand = _commandFactory.CreateFromTask(EditAsync, nameof(AlarmPageViewModel), nameof(EditCommand),
            Observable.Return(true), ui);
        DeleteCommand = _commandFactory.CreateFromTask(DeleteAsync, nameof(AlarmPageViewModel), nameof(DeleteCommand),
            Observable.Return(true), ui);
    }

    public ObservableCollection<AlarmModel> Items { get; } = [];

    [ObservableProperty]
    private AlarmModel? _selectedItem;

    public RxCommand<Unit, Unit> LoadCommand { get; }

    public RxCommand<Unit, Unit> NewCommand { get; }

    public RxCommand<Unit, Unit> EditCommand { get; }

    public RxCommand<Unit, Unit> DeleteCommand { get; }

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
        var items = await _api.AlarmsGetAllAsync();
        Items.Clear();
        foreach (var item in items)
            Items.Add(item);

        SelectedItem = Items.FirstOrDefault();
    }

    private async Task NewAsync()
    {
        var result = await _dialogService.ShowDialogAsync("alarm-edit", new DialogParameters
        {
            { "isEdit", false },
            { "id", 0 },
            { "title", string.Empty },
            { "time", DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) }
        });

        if (result.Result != ButtonResult.OK)
            return;

        var created = result.Parameters.GetValue<AlarmModel>("item")!;
        Items.Add(created);
        SelectedItem = created;
    }

    private async Task EditAsync()
    {
        if (SelectedItem is null)
            return;

        var item = SelectedItem;
        var result = await _dialogService.ShowDialogAsync("alarm-edit", new DialogParameters
        {
            { "isEdit", true },
            { "id", item.Id },
            { "title", item.Title ?? string.Empty },
            { "time", item.Time!.Value.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture) }
        });

        if (result.Result != ButtonResult.OK)
            return;

        var updated = result.Parameters.GetValue<AlarmModel>("item")!;
        item.Title = updated.Title;
        item.Time = updated.Time;
        ReselectCurrentItem();
    }

    private async Task DeleteAsync()
    {
        if (SelectedItem is null)
            return;

        var item = SelectedItem;
        var confirm = await _dialogService.ShowNotificationAsync(
            "Delete Alarm",
            $"Delete \"{item.Title}\"?",
            [
                new DialogButton("Cancel", ButtonResult.Cancel),
                new DialogButton("Delete", ButtonResult.OK)
            ]);

        if (confirm.Result != ButtonResult.OK)
            return;

        await _api.AlarmsDeleteAsync(item.Id);
        Items.Remove(item);
        SelectedItem = Items.FirstOrDefault();
    }

    private void ReselectCurrentItem()
    {
        var current = SelectedItem;
        SelectedItem = null;
        SelectedItem = current;
    }
}
