using System;
using System.Threading.Tasks;
using Marketplace.EventSourcing;
using Marketplace.RavenDb.Logging;
using Raven.Client.Documents.Session;

namespace Marketplace.RavenDb
{
    public class RavenDbProjection<T> : ISubscription
    {
        static readonly ILog Log = LogProvider.GetCurrentClassLogger();
        static readonly string ReadModelName = typeof(T).Name;

        public RavenDbProjection(
            GetSession getSession,
            Projector projector
        )
        {
            _projector = projector;
            GetSession = getSession;
        }

        GetSession GetSession { get; }
        readonly Projector _projector;

        public async Task Project(object @event)
        {
            using var session = GetSession();

            var handler = _projector(session, @event);

            if ( handler == null) return;

            Log.Debug("Projecting {event} to {model}", @event, ReadModelName);

            await handler();
            await session.SaveChangesAsync();
        }

        public delegate Func<Task> Projector(
            IAsyncDocumentSession session,
            object @event
        );
    }
}