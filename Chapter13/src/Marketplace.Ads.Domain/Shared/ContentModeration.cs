using System.Threading.Tasks;

namespace Marketplace.Ads.Domain.Shared
{
    public delegate Task<bool> CheckTextForProfanity(string text);
}