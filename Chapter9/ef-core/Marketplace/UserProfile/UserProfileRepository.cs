using System;
using System.Threading.Tasks;
using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Infrastructure;

namespace Marketplace.UserProfile
{
    public class UserProfileRepository : IUserProfileRepository, IDisposable
    {
        private readonly MarketplaceDbContext _dbContext;

        public UserProfileRepository(MarketplaceDbContext dbContext) 
            => _dbContext = dbContext;

        public Task Add(Domain.UserProfile.UserProfile entity) 
            => _dbContext.UserProfiles.AddAsync(entity);

        public async Task<bool> Exists(UserId id) 
            => await _dbContext.UserProfiles.FindAsync(id.Value) != null;

        public Task<Domain.UserProfile.UserProfile> Load(UserId id)
            => _dbContext.UserProfiles.FindAsync(id.Value);

        public void Dispose() => _dbContext.Dispose();
    }
}
