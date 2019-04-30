using System.Collections.Generic;
using System.Threading.Tasks;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Queries;
using Raven.Client.Documents.Session;
using static Marketplace.ClassifiedAd.QueryModels;
using static Marketplace.ClassifiedAd.ReadModels;
using static Marketplace.Domain.ClassifiedAd.ClassifiedAd;

namespace Marketplace.ClassifiedAd
{
    public static class Queries
    {
        public static Task<List<PublicClassifiedAdListItem>> Query(
            this IAsyncDocumentSession session,
            GetPublishedClassifiedAds query
        ) =>
            session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                .Where(x => x.State == ClassifiedAdState.Active)
                .Select(
                    x =>
                        new PublicClassifiedAdListItem
                        {
                            ClassifiedAdId = x.Id.Value,
                            Price = x.Price.Amount,
                            Title = x.Title.Value,
                            CurrencyCode = x.Price.Currency.CurrencyCode
                        }
                )
                .PagedList(query.Page, query.PageSize);

        public static Task<List<PublicClassifiedAdListItem>> Query(
            this IAsyncDocumentSession session,
            GetOwnersClassifiedAd query
        )
            =>
                session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                    .Where(x => x.OwnerId.Value == query.OwnerId)
                    .Select(
                        x =>
                            new PublicClassifiedAdListItem
                            {
                                ClassifiedAdId = x.Id.Value,
                                Price = x.Price.Amount,
                                Title = x.Title.Value,
                                CurrencyCode = x.Price.Currency.CurrencyCode
                            }
                    )
                    .PagedList(query.Page, query.PageSize);

        public static Task<ClassifiedAdDetails> Query(
            this IAsyncDocumentSession session,
            GetPublicClassifiedAd query
        )
            => (from ad in session.Query<Domain.ClassifiedAd.ClassifiedAd>()
                where ad.Id.Value == query.ClassifiedAdId
                let user = RavenQuery
                    .Load<Domain.UserProfile.UserProfile>(
                        "UserProfile/" + ad.OwnerId.Value
                    )
                select new ClassifiedAdDetails
                {
                    ClassifiedAdId = ad.Id.Value,
                    Title = ad.Title.Value,
                    Description = ad.Text.Value,
                    Price = ad.Price.Amount,
                    CurrencyCode = ad.Price.Currency.CurrencyCode,
                    SellersDisplayName = user.DisplayName.Value
                }).SingleAsync();

        private static Task<List<T>> PagedList<T>(
            this IRavenQueryable<T> query, int page, int pageSize
        ) =>
            query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();
    }
}