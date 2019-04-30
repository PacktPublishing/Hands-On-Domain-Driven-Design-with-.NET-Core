using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Marketplace.ClassifiedAd;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;
using static Marketplace.Domain.ClassifiedAd.Events;
using static Marketplace.Domain.UserProfile.Events;
using static Marketplace.Projections.ClassifiedAdUpcastedEvents;
using static Marketplace.Projections.ReadModels;

namespace Marketplace.Projections
{
    public class ClassifiedAdDetailsProjection
        : RavenDbProjection<ClassifiedAdDetails>
    {
        private readonly Func<Guid, Task<string>>
            _getUserDisplayName;

        public ClassifiedAdDetailsProjection(
            Func<IAsyncDocumentSession> getSession,
            Func<Guid, Task<string>> getUserDisplayName
        )
            : base(getSession)
            => _getUserDisplayName = getUserDisplayName;

        public override Task Project(object @event) =>
            @event switch
            {
                ClassifiedAdCreated e =>
                    Create(async () =>
                        new ClassifiedAdDetails
                        {
                            Id = e.Id.ToString(),
                            SellerId = e.OwnerId,
                            SellersDisplayName =
                                await _getUserDisplayName(
                                    e.OwnerId
                                )
                        }
                    ),
                ClassifiedAdTitleChanged e =>
                    UpdateOne(e.Id, ad => ad.Title = e.Title),
                ClassifiedAdTextUpdated e =>
                    UpdateOne(e.Id, ad => ad.Description = e.AdText),
                ClassifiedAdPriceUpdated e =>
                    UpdateOne(
                        e.Id,
                        ad =>
                        {
                            ad.Price = e.Price;
                            ad.CurrencyCode = e.CurrencyCode;
                        }
                    ),
                UserDisplayNameUpdated e =>
                    UpdateWhere(
                        x => x.SellerId == e.UserId,
                        x => x.SellersDisplayName = e.DisplayName
                    ),
                V1.ClassifiedAdPublished e =>
                    UpdateOne(
                        e.Id,
                        ad => ad.SellersPhotoUrl = e.SellersPhotoUrl
                    ),
                _ => Task.CompletedTask
            };
    }
}