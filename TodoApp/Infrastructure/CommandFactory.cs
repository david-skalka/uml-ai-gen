using System.Diagnostics;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using TodoApp.Services;

namespace TodoApp.Infrastructure;

public interface ICommandFactory
{
    RxCommand<Unit, Unit> Create(Action execute, string viewModelName, string commandName, IScheduler outputScheduler);

    RxCommand<TParam, Unit> Create<TParam>(Action<TParam> execute, string viewModelName, string commandName,
        IScheduler outputScheduler);

    RxCommand<Unit, Unit> CreateFromTask(Func<Task> execute, string viewModelName, string commandName,
        IScheduler outputScheduler);

    RxCommand<Unit, Unit> CreateFromTask(Func<Task> execute, string viewModelName, string commandName,
        IObservable<bool> canExecute, IScheduler outputScheduler);
}

public class CommandFactory(ILogger<CommandFactory> logger, IErrorHandlerService errorHandler) : ICommandFactory
{
    public RxCommand<Unit, Unit> Create(Action execute, string viewModelName, string commandName,
        IScheduler outputScheduler)
    {
        var wrapped = Wrap(execute, viewModelName, commandName);
        return Configure(new RxCommand<Unit, Unit>(
            _ => Observable.Return(Unit.Default).Do(_ => wrapped()),
            Observable.Return(true),
            outputScheduler));
    }

    public RxCommand<TParam, Unit> Create<TParam>(Action<TParam> execute, string viewModelName,
        string commandName, IScheduler outputScheduler)
    {
        var wrapped = Wrap(execute, viewModelName, commandName);
        return Configure(new RxCommand<TParam, Unit>(
            p => Observable.Return(Unit.Default).Do(_ => wrapped(p)),
            Observable.Return(true),
            outputScheduler));
    }

    public RxCommand<Unit, Unit> CreateFromTask(Func<Task> execute, string viewModelName, string commandName,
        IScheduler outputScheduler)
    {
        var wrapped = Wrap(execute, viewModelName, commandName);
        return Configure(new RxCommand<Unit, Unit>(
            _ => Observable.FromAsync(wrapped),
            Observable.Return(true),
            outputScheduler));
    }

    public RxCommand<Unit, Unit> CreateFromTask(Func<Task> execute, string viewModelName, string commandName,
        IObservable<bool> canExecute, IScheduler outputScheduler)
    {
        var wrapped = Wrap(execute, viewModelName, commandName);
        return Configure(new RxCommand<Unit, Unit>(
            _ => Observable.FromAsync(wrapped),
            canExecute,
            outputScheduler));
    }

    private RxCommand<TParam, TResult> Configure<TParam, TResult>(
        RxCommand<TParam, TResult> command)
    {
        command.ThrownExceptions
            .Subscribe(ex => { errorHandler.Handle(ex); });

        return command;
    }

    private Action Wrap(Action execute, string viewModelName, string commandName)
    {
        return () =>
        {
            logger.LogInformation("[COMMAND ENTER] {ViewModelName}.{CommandName}", viewModelName, commandName);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                execute();
                stopwatch.Stop();
                logger.LogInformation("[COMMAND EXIT] {ViewModelName}.{CommandName} | {Elapsed}ms", viewModelName,
                    commandName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                LogFault(ex, viewModelName, commandName, stopwatch);
                throw;
            }
        };
    }

    private Action<TParam> Wrap<TParam>(Action<TParam> execute, string viewModelName, string commandName)
    {
        return parameter =>
        {
            logger.LogInformation("[COMMAND ENTER] {ViewModelName}.{CommandName} | Param={Param}", viewModelName,
                commandName, FormatParameter(parameter));
            var stopwatch = Stopwatch.StartNew();
            try
            {
                execute(parameter);
                stopwatch.Stop();
                logger.LogInformation("[COMMAND EXIT] {ViewModelName}.{CommandName} | {Elapsed}ms", viewModelName,
                    commandName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                LogFault(ex, viewModelName, commandName, stopwatch, parameter);
                throw;
            }
        };
    }

    private Func<Task> Wrap(Func<Task> execute, string viewModelName, string commandName)
    {
        return async () =>
        {
            logger.LogInformation("[COMMAND ENTER] {ViewModelName}.{CommandName}", viewModelName, commandName);
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await execute();
                stopwatch.Stop();
                logger.LogInformation("[COMMAND EXIT] {ViewModelName}.{CommandName} | {Elapsed}ms", viewModelName,
                    commandName, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                LogFault(ex, viewModelName, commandName, stopwatch);
                throw;
            }
        };
    }

    private void LogFault(Exception ex, string viewModelName, string commandName, Stopwatch sw)
    {
        sw.Stop();
        logger.LogError(ex, "[COMMAND FAULT] {ViewModelName}.{CommandName} | {Elapsed}ms | {Message}",
            viewModelName, commandName, sw.ElapsedMilliseconds, ex.Message);
    }

    private void LogFault<TParam>(Exception ex, string viewModelName, string commandName, Stopwatch sw, TParam param)
    {
        sw.Stop();
        logger.LogError(ex,
            "[COMMAND FAULT] {ViewModelName}.{CommandName} | Param={Param} | {Elapsed}ms | {Message}",
            viewModelName, commandName, FormatParameter(param), sw.ElapsedMilliseconds, ex.Message);
    }

    private static string FormatParameter<TParam>(TParam parameter) =>
        parameter switch
        {
            string value => value,
            ValueType value => Convert.ToString(value)!,
            _ => JsonConvert.SerializeObject(parameter)
        };
}
