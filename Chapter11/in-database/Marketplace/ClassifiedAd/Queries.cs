using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using static Marketplace.ClassifiedAd.QueryModels;
using static Marketplace.Projections.ReadModels;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        public static Task<ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            GetPublicClassifiedAd query
        ) =>
            session.LoadAsync<ClassifiedAdDetails>(
                query.ClassifiedAdId.ToString()
            );
    }
}