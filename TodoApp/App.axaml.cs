using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Logging;
using Prism.DryIoc;
using TodoApp.Api;
using TodoApp.Infrastructure;
using TodoApp.Services;
using TodoApp.ViewModels.Alarm;
using TodoApp.ViewModels.Dialogs;
using TodoApp.ViewModels.Shell;
using TodoApp.ViewModels.TodoList;
using TodoApp.Views.Alarm;
using TodoApp.Views.Dialogs;
using TodoApp.Views.Shell;
using TodoApp.Views.TodoList;
using TodoApp.UrsaPrism;

namespace TodoApp;

public class App : PrismApplication
{
    private readonly Client _apiClient;
    private readonly AppOptions _appOptions;

    public App(AppOptions appOptions, Client apiClient, IApplicationLifetime? applicationLifetime)
    {
        _appOptions = appOptions;
        _apiClient = apiClient;
        ApplicationLifetime = applicationLifetime;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        base.Initialize();
    }

    protected override AvaloniaObject CreateShell()
    {
        var window = Container.Resolve<MainWindow>();
        window.DataContext = Container.Resolve<MainWindowViewModel>();
        return window;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        containerRegistry.RegisterInstance(loggerFactory);
        containerRegistry.Register(typeof(ILogger<>), typeof(Logger<>));

        containerRegistry.RegisterInstance(_appOptions);
        containerRegistry.RegisterInstance(_apiClient);
        containerRegistry.RegisterSingleton<IClient>(c => c.Resolve<Client>());
        containerRegistry.RegisterSingleton<IErrorHandlerService, ErrorHandlerService>();
        containerRegistry.RegisterSingleton<ICommandFactory, CommandFactory>();
        containerRegistry.RegisterForNavigation<TodoListPageView, TodoListPageViewModel>();
        containerRegistry.RegisterForNavigation<AlarmPageView, AlarmPageViewModel>();
        containerRegistry.RegisterSingleton<MainWindowViewModel>();
        containerRegistry.Register<MainWindow>();
        containerRegistry.Register<DialogNotificationViewModel>();
        containerRegistry.Register<DialogTodoListEditViewModel>();
        containerRegistry.Register<DialogGroupByNameViewModel>();
        containerRegistry.Register<DialogAlarmEditViewModel>();

        containerRegistry.RegisterUrsaDialogService();
        containerRegistry.RegisterUrsaDialogView<DialogNotificationView>(nameof(DialogNotificationView));
        containerRegistry.RegisterUrsaDialogView<DialogTodoListEditView>(nameof(DialogTodoListEditView));
        containerRegistry.RegisterUrsaDialogView<DialogGroupByNameView>(nameof(DialogGroupByNameView));
        containerRegistry.RegisterUrsaDialogView<DialogAlarmEditView>(nameof(DialogAlarmEditView));
        containerRegistry.RegisterSingleton<IAppDialogService, AppOverlayDialogService>();
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Container.Resolve<IRegionManager>()
            .RequestNavigate(RegionNames.ContentRegion, nameof(TodoListPageView));
    }
}
