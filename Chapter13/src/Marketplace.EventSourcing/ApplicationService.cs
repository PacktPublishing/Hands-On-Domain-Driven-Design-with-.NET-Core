using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Marketplace.EventSourcing.Logging;

namespace Marketplace.EventSourcing
{
    public abstract class ApplicationService<T> where T : AggregateRoot
    {
        readonly Dictionary<Type, Func<object, Task>> _handlers =
            new Dictionary<Type, Func<object, Task>>();

        readonly IAggregateStore _store;

        protected ApplicationService(IAggregateStore store) => _store = store;

        static ILog Log => LogProvider.GetCurrentClassLogger();

        public Task Handle<TCommand>(TCommand command)
        {
            if (!_handlers.TryGetValue(typeof(TCommand), out var handler))
                throw new InvalidOperationException(
                    $"No registered handler for command {typeof(TCommand).Name}"
                );

            Log.DebugFormat("Handling command: {command}", command.ToString());
            return handler(command);
        }

        protected void CreateWhen<TCommand>(
            Func<TCommand, AggregateId<T>> getAggregateId,
            Func<TCommand, AggregateId<T>, T> creator)
            where TCommand : class
            => When<TCommand>(
                async command =>
                {
                    var aggregateId = getAggregateId(command);

                    if (await _store.Exists(aggregateId))
                        throw new InvalidOperationException(
                            $"Entity with id {aggregateId.ToString()} already exists"
                        );

                    var aggregate = creator(command, aggregateId);

                    await _store.Save(aggregate);
                }
            );

        protected void UpdateWhen<TCommand>(
            Func<TCommand, AggregateId<T>> getAggregateId,
            Action<T, TCommand> updater) where TCommand : class
            => When<TCommand>(
                async command =>
                {
                    var aggregateId = getAggregateId(command);
                    var aggregate = await _store.Load(aggregateId);

                    if (aggregate == null)
                        throw new InvalidOperationException(
                            $"Entity with id {aggregateId.ToString()} cannot be found"
                        );

                    updater(aggregate, command);
                    await _store.Save(aggregate);
                }
            );

        protected void UpdateWhen<TCommand>(
            Func<TCommand, AggregateId<T>> getAggregateId,
            Func<T, TCommand, Task> updater) where TCommand : class
            => When<TCommand>(
                async command =>
                {
                    var aggregateId = getAggregateId(command);
                    var aggregate = await _store.Load(aggregateId);

                    if (aggregate == null)
                        throw new InvalidOperationException(
                            $"Entity with id {aggregateId.ToString()} cannot be found"
                        );

                    await updater(aggregate, command);
                    await _store.Save(aggregate);
                }
            );

        void When<TCommand>(Func<TCommand, Task> handler)
            where TCommand : class
            => _handlers.Add(typeof(TCommand), c => handler((TCommand) c));
    }
}