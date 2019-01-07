using System.Threading.Tasks;
using Marketplace.Domain.Shared;

namespace Marketplace.Domain.UserProfile
{
    public interface IUserProfileRepository
    {
        Task<UserProfile> Load(UserId id);

        Task Add(UserProfile entity);
        
        Task<bool> Exists(UserId id);
    }
}