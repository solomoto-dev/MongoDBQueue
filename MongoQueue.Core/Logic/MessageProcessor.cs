using System;
using System.Diagnostics;
using System.Threading;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.LogicAbstractions;
using Newtonsoft.Json;

namespace MongoQueue.Core.Logic
{
    public class MessageProcessor
    {
        private readonly IMessageHandlersCache _messageHandlersCache;
        private readonly IMessageTypesCache _messageTypesCache;
        private readonly IMessageHandlerFactory _messageHandlerFactory;
        private readonly IMessagingLogger _messagingLogger;

        public MessageProcessor(
            IMessageHandlersCache messageHandlersCache,
            IMessageTypesCache messageTypesCache,
            IMessageHandlerFactory messageHandlerFactory,
            IMessagingLogger messagingLogger
        )
        {
            _messageHandlersCache = messageHandlersCache;
            _messageTypesCache = messageTypesCache;
            _messageHandlerFactory = messageHandlerFactory;
            _messagingLogger = messagingLogger;
        }

        public async void Process(string route, string messageId, string topic, string payload, bool resend,
            CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                var type = _messageTypesCache.Get(topic);
                var message = JsonConvert.DeserializeObject(payload, type);
                var handlerType = _messageHandlersCache.Get(topic);
                var handlerInstance = _messageHandlerFactory.Create(handlerType);
                await handlerInstance.Handle(route, messageId, message, resend, cancellationToken);
            }
            catch (Exception e)
            {
                _messagingLogger.Error(e);
            }
            finally
            {
                _messagingLogger.Debug($"{messageId} processed in {sw.ElapsedMilliseconds}");
            }
        }
    }
}