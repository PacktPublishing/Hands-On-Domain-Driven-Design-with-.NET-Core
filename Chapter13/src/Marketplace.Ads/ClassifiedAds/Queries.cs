using System.Threading.Tasks;
using Marketplace.Ads.Projections;
using Raven.Client.Documents.Session;

namespace Marketplace.Modules.ClassifiedAds
{
    public static class Queries
    {
        public static Task<ReadModels.ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetPublicClassifiedAd query)
            => session.LoadAsync<ReadModels.ClassifiedAdDetails>(
                query.ClassifiedAdId.ToString()
            );
    }
}