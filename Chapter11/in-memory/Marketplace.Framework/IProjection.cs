using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public interface IProjection
    {
        Task Project(object @event);
    }
}