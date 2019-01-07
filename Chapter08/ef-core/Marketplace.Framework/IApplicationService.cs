using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IApplicationService
    {
        Task Handle(object command);
    }
}