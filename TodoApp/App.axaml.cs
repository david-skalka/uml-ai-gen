using System.Net.Http;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
using Microsoft.Extensions.Logging;
using Prism.DryIoc;
using Prism.Ioc;

namespace TodoApp;

public class App : PrismApplication
{
    private readonly AppOptions _appOptions;
    private readonly Client _apiClient;

    public App(AppOptions appOptions, Client apiClient)
    {
        _appOptions = appOptions;
        _apiClient = apiClient;
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
        containerRegistry.RegisterSingleton<TodoListPageViewModel>();
        containerRegistry.RegisterSingleton<AlarmPageViewModel>();
        containerRegistry.RegisterSingleton<MainWindowViewModel>();
        containerRegistry.Register<MainWindow>();

        containerRegistry.RegisterDialogWindow<MyDialogWindow>();
        containerRegistry.RegisterDialog<DialogNotificationView, DialogNotificationViewModel>("notification");
        containerRegistry.RegisterDialog<DialogTodoListEditView, DialogTodoListEditViewModel>("todo-list-edit");
        containerRegistry.RegisterDialog<DialogAlarmEditView, DialogAlarmEditViewModel>("alarm-edit");
    }

    public override void OnFrameworkInitializationCompleted()
    {
        base.OnFrameworkInitializationCompleted();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow?.DataContext is MainWindowViewModel viewModel)
        {
            _ = viewModel.InitializeAsync();
        }
    }
}
