using Avalonia.Threading;
using FluentAssertions;
using FluentAssertions.Extensions;
using FluentAssertions.Primitives;
using static FluentAssertions.FluentActions;

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
        Invoking(() =>
        {
            Dispatcher.UIThread.RunJobs();
            check(subject);
        }).Should().NotThrowAfter(wait, poll);
    }
}
