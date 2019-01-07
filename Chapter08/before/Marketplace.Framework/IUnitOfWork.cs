using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IUnitOfWork
    {
        Task Commit();
    }
}