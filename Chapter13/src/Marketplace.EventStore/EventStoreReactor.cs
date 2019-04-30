using System;
using System.Threading.Tasks;
using Marketplace.EventSourcing;
using Marketplace.EventStore.Logging;

namespace Marketplace.EventStore
{
    public abstract class ReactorBase : ISubscription
    {
        static readonly ILog Log = LogProvider.GetCurrentClassLogger();
        
        public ReactorBase(Reactor reactor) => _reactor = reactor;

        readonly Reactor _reactor;

        public Task Project(object @event)
        {
            var handler = _reactor(@event);

            if (handler == null) return Task.CompletedTask;
            
            Log.Debug("Reacting to event {event}", @event);

            return handler();
        }
        
        public delegate Func<Task> Reactor(object @event);
    }
}