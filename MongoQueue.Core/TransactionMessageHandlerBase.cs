using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using MongoQueue.Core.AgentAbstractions;
using MongoQueue.Core.LogicAbstractions;

namespace MongoQueue.Core
{
    public abstract class TransactionMessageHandlerBase<T> : MongoDependentHandlerBase<T>, IHandler<T>
    {
        protected TransactionMessageHandlerBase(IMessageStatusManager messageStatusManager) : base(messageStatusManager)
        {
        }

        public abstract Task HandleInTransaction(T message, bool resend, CancellationToken cancellationToken);

        public override async Task Handle(T message, bool resend, CancellationToken cancellationToken)
        {
            var transactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
            };
            using (var transaction = new TransactionScope(TransactionScopeOption.Required, transactionOptions, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    await HandleInTransaction(message, resend, cancellationToken);
                    transaction.Complete();
                }
                catch
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }
    }
}