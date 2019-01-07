using System.Threading.Tasks;
using Marketplace.Framework;

namespace Marketplace.Infrastructure
{
    public class EfCoreUnitOfWork : IUnitOfWork
    {
        private readonly ClassifiedAdDbContext _dbContext;

        public EfCoreUnitOfWork(ClassifiedAdDbContext dbContext)
            => _dbContext = dbContext;

        public Task Commit() => _dbContext.SaveChangesAsync();
    }
}