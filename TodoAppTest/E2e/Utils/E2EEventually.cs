using Avalonia.Threading;
using FluentAssertions;
using FluentAssertions.Extensions;

namespace TodoAppTest.E2e.Utils;

public static class E2EEventually
{
    private static readonly TimeSpan DefaultWait = 10.Seconds();
    private static readonly TimeSpan DefaultPoll = 50.Milliseconds();

    public static void Assert(Action check, TimeSpan? wait = null, TimeSpan? poll = null)
    {
        Action runCheck = () =>
        {
            Dispatcher.UIThread.RunJobs();
            check();
        };

        runCheck.Should().NotThrowAfter(wait ?? DefaultWait, poll ?? DefaultPoll);
    }
}
