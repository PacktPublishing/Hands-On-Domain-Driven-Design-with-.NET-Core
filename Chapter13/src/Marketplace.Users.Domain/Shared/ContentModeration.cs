using System.Threading.Tasks;

namespace Marketplace.Users.Domain.Shared
{
    public delegate Task<bool> CheckTextForProfanity(string text);
}