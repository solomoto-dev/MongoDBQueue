using System;
using System.Diagnostics;
using System.Threading;
using Autofac;
using MongoQueue.Core.IntegrationAbstractions;
using MongoQueue.Core.IoC;
using MongoQueue.Core.LogicAbstractions;
using Newtonsoft.Json;

namespace MongoQueue.Core.Logic
{
    public class MessageProcessor
    {
        private readonly IMessageHandlersCache _messageHandlersCache;
        private readonly IMessageTypesCache _messageTypesCache;
        private readonly IMessagingLogger _messagingLogger;
        private readonly IInstanceResolver _instanceResolver;

        public MessageProcessor(
            IMessageHandlersCache messageHandlersCache,
            IMessageTypesCache messageTypesCache,
            IMessagingLogger messagingLogger,
            IInstanceResolver instanceResolver
        )
        {
            _messageHandlersCache = messageHandlersCache;
            _messageTypesCache = messageTypesCache;
            _messagingLogger = messagingLogger;
            _instanceResolver = instanceResolver;
        }

        public async void Process(string route, string messageId, string topic, string payload, bool resend,
            CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                using (var scope = _instanceResolver.CreateLifeTimeScope())
                {
                    scope.ServiceProvider.GetService(typeof(ICurrentHandlerScopeHolder));
                    var type = _messageTypesCache.Get(topic);
                    var message = JsonConvert.DeserializeObject(payload, type);
                    var handlerType = _messageHandlersCache.Get(topic);
                    var handlerInstance = (IHandler)scope.ServiceProvider.GetService(handlerType);
                    await handlerInstance.Handle(route, messageId, message, resend, cancellationToken);
                }
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