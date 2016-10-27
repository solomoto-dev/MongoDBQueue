using System;
using System.Threading;
using System.Threading.Tasks;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.IntegrationAbstractions;

namespace MongoQueue.Core.Logic
{
    public class UnprocessedMessagesResender
    {
        private readonly IUnprocessedMessagesAgent _unprocessedMessagesAgent;
        private readonly IMessagingLogger _messagingLogger;

        public UnprocessedMessagesResender(
            IUnprocessedMessagesAgent unprocessedMessagesAgent,
            IMessagingLogger messagingLogger
        )
        {
            _unprocessedMessagesAgent = unprocessedMessagesAgent;
            _messagingLogger = messagingLogger;
        }

        public async void Start(string appName, TimeSpan resendInterval, CancellationToken cancellationToken)
        {
            try
            {
                await RepeatEvery(() => Resend(appName, cancellationToken), resendInterval);
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"{appName} error on UnprocessedMessagesResender.Start");
            }
        }

        private async Task RepeatEvery(Func<Task> action, TimeSpan interval)
        {
            while (true)
            {
                try
                {
                    await action();
                    await Task.Delay(interval);
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task Resend(string appName, CancellationToken cancellationToken)
        {
            try
            {
                var notProcessed = await _unprocessedMessagesAgent.GetUnprocessed(appName, cancellationToken);
                foreach (var envelope in notProcessed)
                {
                    var resendId = await _unprocessedMessagesAgent.Resend(appName, envelope, cancellationToken);
                    _messagingLogger.Trace($"{appName} resent {envelope.Id} as {resendId}");
                }
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"{appName} resend error");
            }
        }
    }
}