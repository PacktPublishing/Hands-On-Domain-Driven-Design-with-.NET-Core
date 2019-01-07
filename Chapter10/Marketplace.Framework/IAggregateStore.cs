using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IAggregateStore
    {
        Task<bool> Exists<T, TId>(TId aggregateId);
        
        Task Save<T, TId>(T aggregate) where T : AggregateRoot<TId>;
        
        Task<T> Load<T, TId>(TId aggregateId) where T : AggregateRoot<TId>;
    }
}