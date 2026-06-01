using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace TodoApp.Infrastructure;

public sealed class RxCommand<TParam, TResult> : ICommand
{
    private readonly Func<TParam, IObservable<TResult>> _execute;
    private readonly IScheduler _outputScheduler;
    private readonly CompositeDisposable _subs = new();
    private readonly Subject<Exception> _thrownExceptions = new();
    private bool _canRun = true;

    public RxCommand(Func<TParam, IObservable<TResult>> execute, IObservable<bool> canExecute,
        IScheduler outputScheduler)
    {
        _execute = execute;
        _outputScheduler = outputScheduler;
        _subs.Add(canExecute.ObserveOn(_outputScheduler).Subscribe(b =>
        {
            _canRun = b;
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }));
    }

    public IObservable<Exception> ThrownExceptions => _thrownExceptions.AsObservable();

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter)
    {
        return _canRun;
    }

    public void Execute(object? parameter)
    {
        var p = ConvertParameter(parameter);
        Execute(p).Subscribe(_ => { }, ex => _thrownExceptions.OnNext(ex));
    }

    public IObservable<TResult> Execute(TParam parameter = default!)
    {
        return Observable.Defer(() =>
            _execute(parameter)
                .ObserveOn(_outputScheduler)
                .Catch<TResult, Exception>(ex =>
                {
                    _thrownExceptions.OnNext(ex);
                    return Observable.Empty<TResult>();
                }));
    }

    private static TParam ConvertParameter(object? parameter)
    {
        if (parameter is TParam p)
            return p;
        if (typeof(TParam) == typeof(Unit))
            return default!;
        return (TParam)parameter!;
    }
}