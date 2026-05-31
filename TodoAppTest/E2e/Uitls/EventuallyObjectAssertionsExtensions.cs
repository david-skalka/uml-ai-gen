using Avalonia.Threading;
using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Extensions;
using FluentAssertions.Primitives;

namespace TodoAppTest.E2e.Uitls;

public static class EventuallyObjectAssertionsExtensions
{
    private static readonly TimeSpan DefaultWait = 10.Seconds();
    private static readonly TimeSpan DefaultPoll = 50.Milliseconds();

    public static void EventuallySatisfy<TSubject, TAssertions>(
        this ObjectAssertions<TSubject, TAssertions> assertions,
        Action check)
        where TAssertions : ObjectAssertions<TSubject, TAssertions> =>
        assertions.EventuallySatisfy(check, DefaultWait, DefaultPoll);

    public static void EventuallySatisfy<TSubject, TAssertions>(
        this ObjectAssertions<TSubject, TAssertions> assertions,
        Action<TSubject> check)
        where TAssertions : ObjectAssertions<TSubject, TAssertions> =>
        assertions.EventuallySatisfy(check, DefaultWait, DefaultPoll);

    public static void EventuallySatisfy<TSubject, TAssertions>(
        this ObjectAssertions<TSubject, TAssertions> assertions,
        Action check,
        TimeSpan wait,
        TimeSpan poll)
        where TAssertions : ObjectAssertions<TSubject, TAssertions> =>
        assertions.EventuallySatisfy(_ => check(), wait, poll);

    public static void EventuallySatisfy<TSubject, TAssertions>(
        this ObjectAssertions<TSubject, TAssertions> assertions,
        Action<TSubject> check,
        TimeSpan wait,
        TimeSpan poll)
        where TAssertions : ObjectAssertions<TSubject, TAssertions>
    {
        var subject = assertions.Subject;
        var deadline = DateTime.UtcNow + wait;
        Exception? last = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                Dispatcher.UIThread.RunJobs();
                RunCheck(() => check(subject));
                return;
            }
            catch (Exception ex)
            {
                last = ex;
                WaitWithUiPump(poll);
            }
        }

        throw last ?? new TimeoutException();
    }

    private static void RunCheck(Action check)
    {
        string[] failures;
        using (var scope = new AssertionScope())
        {
            check();
            failures = scope.Discard();
        }

        if (failures.Length > 0)
            throw new AssertionFailedException(string.Join(Environment.NewLine, failures));
    }

    private static void WaitWithUiPump(TimeSpan duration)
    {
        if (!Dispatcher.UIThread.CheckAccess())
        {
            Thread.Sleep(duration);
            return;
        }

        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer { Interval = duration };
        timer.Tick += (_, _) =>
        {
            frame.Continue = false;
            timer.Stop();
        };
        timer.Start();
        Dispatcher.UIThread.PushFrame(frame);
    }
}
