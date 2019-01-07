using System;
using System.Threading.Tasks;
using Marketplace.Framework;
using Raven.Client.Documents.Session;

namespace Marketplace.Infrastructure
{
    public class RavenDbRepository<T, TId>
        where T : AggregateRoot<TId>
        where TId : Value<TId>
    {
        private readonly IAsyncDocumentSession _session;
        private readonly Func<TId, string> _entityId;

        public RavenDbRepository(
            IAsyncDocumentSession session,
            Func<TId, string> entityId)
        {
            _session = session;
            _entityId = entityId;
        }

        public Task Add(T entity)
            => _session.StoreAsync(entity, _entityId(entity.Id));

        public Task<bool> Exists(TId id)
            => _session.Advanced.ExistsAsync(_entityId(id));

        public Task<T> Load(TId id)
            => _session.LoadAsync<T>(_entityId(id));
    }
}