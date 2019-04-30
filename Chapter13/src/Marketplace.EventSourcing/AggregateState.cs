using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Force.DeepCloner;

namespace Marketplace.EventSourcing
{
    public interface IAggregateState<T>
    {
        T When(T state, object @event);

        string GetStreamName(Guid id);

        string StreamName { get; }

        long Version { get; }
    }

    public abstract class AggregateState<T> : IAggregateState<T>
        where T : class, new()
    {
        public abstract T When(T state, object @event);
        
        public string GetStreamName(Guid id) => $"{typeof(T).Name}-{id:N}";

        public string StreamName => GetStreamName(Id);

        public long Version { get; protected set; }

        public Guid Id { get; protected set; }

        protected T With(T state, Action<T> update)
        {
            update(state);
            return state;
        }

        protected abstract bool EnsureValidState(T newState);

        T Apply(T state, object @event)
        {
            var newState = state.DeepClone();
            newState = When(newState, @event);

            if (!EnsureValidState(newState))
                throw new InvalidEntityState(
                    this, "Post-checks failed"
                );

            return newState;
        }

        public class Result
        {
            public T State { get; }
            public IEnumerable<object> Events { get; }

            public Result(T state, IEnumerable<object> events)
            {
                State = state;
                Events = events;
            }
        }

        public Result NoEvents() => new Result(this as T, new List<object>());

        public Result Apply(params object[] events)
        {
            var newState = this as T;
            newState = events.Aggregate(newState, Apply);
            return new Result(newState, events);
        }

        class InvalidEntityState : Exception
        {
            public InvalidEntityState(object entity, string message)
                : base(
                    $"Entity {entity.GetType().Name} " +
                    $"state change rejected, {message}"
                ) { }
        }
    }
}