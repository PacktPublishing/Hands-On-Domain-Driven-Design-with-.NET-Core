using System;

namespace Marketplace.Framework
{
    public abstract class Entity<TId> : IInternalEventHandler
        where TId : Value<TId>
    {
        private readonly Action<object> _applier;
        
        public TId Id { get; protected set; }

        protected Entity(Action<object> applier) => _applier = applier;
        
        protected Entity() {}

        protected abstract void When(object @event);

        protected void Apply(object @event)
        {
            When(@event);
            _applier(@event);
        }

        void IInternalEventHandler.Handle(object @event) => When(@event);
    }
}