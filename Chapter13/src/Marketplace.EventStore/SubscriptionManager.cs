using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.EventSourcing;
using Marketplace.EventStore.Logging;

namespace Marketplace.EventStore
{
    public class SubscriptionManager
    {
        static readonly ILog Log = LogProvider.GetCurrentClassLogger();

        readonly ICheckpointStore _checkpointStore;
        readonly string _name;
        readonly IEventStoreConnection _connection;
        readonly ISubscription[] _subscriptions;
        EventStoreAllCatchUpSubscription _subscription;

        public SubscriptionManager(
            IEventStoreConnection connection,
            ICheckpointStore checkpointStore,
            string name,
            params ISubscription[] subscriptions)
        {
            _connection = connection;
            _checkpointStore = checkpointStore;
            _name = name;
            _subscriptions = subscriptions;
        }

        public async Task Start()
        {
            var settings = new CatchUpSubscriptionSettings(
                2000, 500,
                Log.IsDebugEnabled(),
                false, _name
            );

            Log.Debug("Starting the projection manager...");

            var position = await _checkpointStore.GetCheckpoint();
            Log.Debug("Retrieved the checkpoint: {checkpoint}", position);

            _subscription = _connection.SubscribeToAllFrom(
                GetPosition(),
                settings, 
                EventAppeared
            );
            Log.Debug("Subscribed to $all stream");

            Position? GetPosition()
                => position.HasValue
                    ? new Position(position.Value, position.Value)
                    : AllCheckpoint.AllStart;
        }

        async Task EventAppeared(
            EventStoreCatchUpSubscription _,
            ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return;

            var @event = resolvedEvent.Deserialze();

            Log.Debug("Projecting event {event}", @event.ToString());

            try
            {
                await Task.WhenAll(_subscriptions.Select(x => x.Project(@event)));

                await _checkpointStore.StoreCheckpoint(
                    // ReSharper disable once PossibleInvalidOperationException
                    resolvedEvent.OriginalPosition.Value.CommitPosition
                );
            }
            catch (Exception e)
            {
                Log.Error(
                    e,
                    "Error occured when projecting the event {event}",
                    @event
                );
                throw;
            }
        }

        public void Stop() => _subscription.Stop();
    }
}