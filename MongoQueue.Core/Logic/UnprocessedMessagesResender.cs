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

        public async void Start(string route, TimeSpan resendInterval, CancellationToken cancellationToken)
        {
            try
            {
                await RepeatEvery(() => Resend(route, cancellationToken), resendInterval);
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"{route} error on UnprocessedMessagesResender.Start");
            }
        }

        private async Task RepeatEvery(Func<Task> action, TimeSpan interval)
        {
            while (true)
            {
                try
                {
                    _messagingLogger.Debug("starting resending cycle");
                    await action();
                    await Task.Delay(interval);
                    _messagingLogger.Debug("resending cycle ended");
                }
                catch (TaskCanceledException)
                {
                    return;
                }
            }
        }

        private async Task Resend(string route, CancellationToken cancellationToken)
        {
            try
            {
                var notProcessed = await _unprocessedMessagesAgent.GetUnprocessed(route, cancellationToken);
                foreach (var envelope in notProcessed)
                {
                    var resendId = await _unprocessedMessagesAgent.Resend(route, envelope, cancellationToken);
                    _messagingLogger.Debug($"{route} resent {envelope.Id} as {resendId}");
                }
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e, $"{route} resend error");
            }
        }
    }
}