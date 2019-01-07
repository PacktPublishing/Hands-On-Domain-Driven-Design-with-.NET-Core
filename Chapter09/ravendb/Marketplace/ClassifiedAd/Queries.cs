using System.Collections.Generic;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        public static Task<List<ReadModels.PublicClassifiedAdListItem>> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetPublishedClassifiedAds query)
            =>
                session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                    .Where(x => x.State == Domain.ClassifiedAd.ClassifiedAd.ClassifiedAdState.Active)
                    .Select(x =>
                        new ReadModels.PublicClassifiedAdListItem
                        {
                            ClassifiedAdId = x .Id.Value,
                            Price = x .Price.Amount,
                            Title = x .Title.Value,
                            CurrencyCode = x .Price.Currency.CurrencyCode
                        })
                    .PagedList(query.Page, query.PageSize);

        public static Task<List<ReadModels.PublicClassifiedAdListItem>> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetOwnersClassifiedAd query)
            =>
                session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                    .Where(x => x.OwnerId.Value == query.OwnerId)
                    .Select(x =>
                        new ReadModels.PublicClassifiedAdListItem
                        {
                            ClassifiedAdId = x .Id.Value,
                            Price = x .Price.Amount,
                            Title = x .Title.Value,
                            CurrencyCode = x .Price.Currency.CurrencyCode
                        })
                    .PagedList(query.Page, query.PageSize);

        public static Task<ReadModels.ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            QueryModels.GetPublicClassifiedAd query)
            => (from ad in session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                where ad.Id.Value == query.ClassifiedAdId
                let user = RavenQuery
                    .Load<Domain.UserProfile.UserProfile>("UserProfile/" + ad.OwnerId.Value)
                select new ReadModels.ClassifiedAdDetails
                {
                    ClassifiedAdId = ad.Id.Value,
                    Title = ad.Title.Value,
                    Description = ad.Text.Value,
                    Price = ad.Price.Amount,
                    CurrencyCode = ad.Price.Currency.CurrencyCode,
                    SellersDisplayName = user.DisplayName.Value
                }).SingleAsync();

        private static Task<List<T>> PagedList<T>(this IRavenQueryable<T> query, int page, int pageSize)
            =>
                query
                    .Skip(page * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

    }
}