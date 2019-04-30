using System;
using System.Threading.Tasks;
using Marketplace.RavenDb;
using Raven.Client.Documents.Session;
using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.Ads.Projections.ReadModels;

namespace Marketplace.Ads.Projections
{
    public static class ClassifiedAdDetailsProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            return @event switch
            {
                V1.ClassifiedAdCreated e =>
                    () => Create(e.Id, e.OwnerId),
                V1.ClassifiedAdTitleChanged e =>
                    () => Update(e.Id, ad => ad.Title = e.Title),
                V1.ClassifiedAdTextUpdated e =>
                    () => Update(e.Id, ad => ad.Description = e.AdText),
                V1.ClassifiedAdPriceUpdated e =>
                    () => Update( e.Id,
                        ad =>
                        {
                            ad.Price = e.Price;
                            ad.CurrencyCode = e.CurrencyCode;
                        }),
                V1.PictureAddedToAClassifiedAd e =>
                    () => Update(e.ClassifiedAdId, 
                        ad => ad.PhotoUrls.Add(e.Url)),
                V1.ClassifiedAdDeleted e =>
                    () => Delete(e.Id),
                _ => (Func<Task>) null
            };

            string GetDbId(Guid id) 
                => ClassifiedAdDetails.GetDatabaseId(id);

            Task Create(Guid id, Guid ownerId)
                => session.Create<ClassifiedAdDetails>(
                    x =>
                    {
                        x.Id = GetDbId(id);
                        x.SellerId = ownerId;
                    }
                );

            Task Update(Guid id, Action<ClassifiedAdDetails> update)
                => session.Update(GetDbId(id), update);

            Task Delete(Guid id)
            {
                session.Delete(GetDbId(id));
                return Task.CompletedTask;
            }
        }
    }
}