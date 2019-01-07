using System;
using System.Threading.Tasks;
using Marketplace.Domain;

namespace Marketplace.Infrastructure
{
    public class ClassifiedAdRepository : IClassifiedAdRepository, IDisposable
    {
        private readonly ClassifiedAdDbContext _dbContext;

        public ClassifiedAdRepository(ClassifiedAdDbContext dbContext) 
            => _dbContext = dbContext;

        public Task Add(ClassifiedAd entity) 
            => _dbContext.ClassifiedAds.AddAsync(entity);

        public async Task<bool> Exists(ClassifiedAdId id) 
            => await _dbContext.ClassifiedAds.FindAsync(id.Value) != null;

        public Task<ClassifiedAd> Load(ClassifiedAdId id)
            => _dbContext.ClassifiedAds.FindAsync(id.Value);

        public void Dispose() => _dbContext.Dispose();
    }
}