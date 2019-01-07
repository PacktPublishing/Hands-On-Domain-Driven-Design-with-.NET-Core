using System.Linq;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework;
using Serilog;
using Serilog.Events;

namespace Marketplace.Infrastructure
{
    public class ProjectionManager
    {
        private readonly IEventStoreConnection _connection;
        private readonly IProjection[] _projections;
        private EventStoreAllCatchUpSubscription _subscription;

        public ProjectionManager(IEventStoreConnection connection, params IProjection[] projections)
        {
            _connection = connection;
            _projections = projections;
        }

        public void Start()
        {
            var settings = new CatchUpSubscriptionSettings(2000, 500,
                Log.IsEnabled(LogEventLevel.Verbose),
                false, "try-out-subscription");
            _subscription = _connection.SubscribeToAllFrom(Position.Start,
                settings, EventAppeared);
        }

        public void Stop() => _subscription.Stop();

        private Task EventAppeared(EventStoreCatchUpSubscription _, ResolvedEvent resolvedEvent)
        {
            if (resolvedEvent.Event.EventType.StartsWith("$")) return Task.CompletedTask;
            
            var @event = resolvedEvent.Deserialzie();
            
            Log.Debug("Projecting event {type}", @event.GetType().Name);
            return Task.WhenAll(_projections.Select(x => x.Project(@event)));
        }
    }
}