using System;
using System.Linq.Expressions;
using System.Threading;
using Moq;
using NUnit.Framework;

namespace MongoQueueTests.Common
{
    public static class Throttle
    {
        public static void Verify<T>(Mock<T> mock, Expression<Action<T>> verification,
            TimeSpan minTimeout = default(TimeSpan), TimeSpan maxTimeout = default(TimeSpan)) where T : class
        {
            maxTimeout = maxTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(5) : maxTimeout;
            var cancellationToken = new CancellationTokenSource(maxTimeout).Token;
            if (minTimeout != default(TimeSpan))
            {
                Thread.Sleep(minTimeout);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    mock.Verify(verification);
                    NUnit.Framework.Assert.True(true, "method was called");
                    return;
                }
                catch (MockException)
                {
                }
                Thread.Sleep(10);
            }
            throw new ThrottleException();
        }

        public static void Assert(Func<bool> action, TimeSpan minTimeout = default(TimeSpan),
            TimeSpan maxTimeout = default(TimeSpan))
        {
            maxTimeout = maxTimeout == default(TimeSpan) ? TimeSpan.FromSeconds(5) : maxTimeout;
            var cancellationToken = new CancellationTokenSource(maxTimeout).Token;
            if (minTimeout != default(TimeSpan))
            {
                Thread.Sleep(minTimeout);
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    NUnit.Framework.Assert.True(action());
                    return;
                }
                catch (AssertionException)
                {
                }
                Thread.Sleep(10);
            }
            throw new ThrottleException();
        }
    }
}

