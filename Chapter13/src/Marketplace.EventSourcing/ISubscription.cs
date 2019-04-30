using System.Threading.Tasks;

namespace Marketplace.EventSourcing
{
    public interface ISubscription
    {
        Task Project(object @event);
    }
}