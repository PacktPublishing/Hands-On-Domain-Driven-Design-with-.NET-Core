using System;
using System.Threading.Tasks;
using Marketplace.Domain.ClassifiedAd;
using Marketplace.Infrastructure;

namespace Marketplace.ClassifiedAd
{
    public class ClassifiedAdRepository : IClassifiedAdRepository, IDisposable
    {
        private readonly MarketplaceDbContext _dbContext;

        public ClassifiedAdRepository(MarketplaceDbContext dbContext) 
            => _dbContext = dbContext;

        public Task Add(Domain.ClassifiedAd.ClassifiedAd entity) 
            => _dbContext.ClassifiedAds.AddAsync(entity);

        public async Task<bool> Exists(ClassifiedAdId id) 
            => await _dbContext.ClassifiedAds.FindAsync(id.Value) != null;

        public Task<Domain.ClassifiedAd.ClassifiedAd> Load(ClassifiedAdId id)
            => _dbContext.ClassifiedAds.FindAsync(id.Value);

        public void Dispose() => _dbContext.Dispose();
    }
}