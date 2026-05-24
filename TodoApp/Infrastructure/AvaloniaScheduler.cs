using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Avalonia.Threading;

namespace TodoApp.Infrastructure;

public sealed class AvaloniaScheduler : IScheduler
{
    public static AvaloniaScheduler Instance { get; } = new();

    public DateTimeOffset Now => DateTimeOffset.Now;

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        var m = new MultipleAssignmentDisposable();
        Dispatcher.UIThread.Post(() =>
        {
            if (!m.IsDisposed)
                m.Disposable = action(this, state);
        }, DispatcherPriority.Normal);
        return m;
    }

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime,
        Func<IScheduler, TState, IDisposable> action) =>
        dueTime <= TimeSpan.Zero ? Schedule(state, action) : Scheduler.Default.Schedule(state, dueTime, (_, s) => Schedule(s, action));

    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime,
        Func<IScheduler, TState, IDisposable> action) =>
        dueTime <= Now ? Schedule(state, action) : Scheduler.Default.Schedule(state, dueTime, (_, s) => Schedule(s, action));
}