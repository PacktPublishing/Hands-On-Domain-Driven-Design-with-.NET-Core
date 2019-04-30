using System;
using System.Threading.Tasks;
using Marketplace.Domain;
using Raven.Client.Documents.Session;

namespace Marketplace
{
    public class ClassifiedAdRepository 
        : IClassifiedAdRepository, IDisposable
    {
        private readonly IAsyncDocumentSession _session;

        public ClassifiedAdRepository(IAsyncDocumentSession session) 
            => _session = session;

        public Task<bool> Exists(ClassifiedAdId id) 
            => _session.Advanced.ExistsAsync(EntityId(id));

        public Task<ClassifiedAd> Load(ClassifiedAdId id)
            => _session.LoadAsync<ClassifiedAd>(EntityId(id));

        public async Task Save(ClassifiedAd entity)
        {
            await _session.StoreAsync(entity, EntityId(entity.Id));
            await _session.SaveChangesAsync();
        }

        public void Dispose() => _session.Dispose();
        
        private static string EntityId(ClassifiedAdId id)
            => $"ClassifiedAd/{id}";
    }
}