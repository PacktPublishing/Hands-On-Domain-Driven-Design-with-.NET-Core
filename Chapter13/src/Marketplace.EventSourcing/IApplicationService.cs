using System.Threading.Tasks;

namespace Marketplace.EventSourcing
{
    public interface IApplicationService
    {
        Task Handle(object command);
    }
}