using System.Threading.Tasks;

namespace Marketplace.Domain.Shared
{
    public delegate Task<bool> CheckTextForProfanity(string text);
}