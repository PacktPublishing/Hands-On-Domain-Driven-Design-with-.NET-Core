using System.Threading.Tasks;

namespace Marketplace.EventSourcing
{
    public interface ICheckpointStore
    {
        Task<long?> GetCheckpoint();
        Task StoreCheckpoint(long? checkpoint);
    }
}