using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Framework;
using static Marketplace.Domain.ClassifiedAd.Events;
using static Marketplace.Domain.UserProfile.Events;
using static Marketplace.Projections.ClassifiedAdUpcastedEvents;
using static Marketplace.Projections.ReadModels;

namespace Marketplace.Projections
{
    public class ClassifiedAdDetailsProjection : IProjection
    {
        private readonly List<ClassifiedAdDetails> _items;
        private readonly Func<Guid, string> _getUserDisplayName;

        public ClassifiedAdDetailsProjection(
            List<ClassifiedAdDetails> items,
            Func<Guid, string> getUserDisplayName
        )
        {
            _items = items;
            _getUserDisplayName = getUserDisplayName;
        }

        public Task Project(object @event)
        {
            switch (@event)
            {
                case ClassifiedAdCreated e:
                    _items.Add(
                        new ClassifiedAdDetails
                        {
                            ClassifiedAdId = e.Id,
                            SellerId = e.OwnerId,
                            SellersDisplayName = 
                                _getUserDisplayName(e.OwnerId)
                        }
                    );
                    break;
                case ClassifiedAdTitleChanged e:
                    UpdateItem(e.Id, ad => ad.Title = e.Title);
                    break;
                case ClassifiedAdTextUpdated e:
                    UpdateItem(e.Id, ad => ad.Description = e.AdText);
                    break;
                case ClassifiedAdPriceUpdated e:
                    UpdateItem(
                        e.Id, ad =>
                        {
                            ad.Price = e.Price;
                            ad.CurrencyCode = e.CurrencyCode;
                        }
                    );
                    break;
                case UserDisplayNameUpdated e:
                    UpdateMultipleItems(
                        x => x.SellerId == e.UserId,
                        x => x.SellersDisplayName = e.DisplayName
                    );
                    break;
                case V1.ClassifiedAdPublished e:
                    UpdateItem(e.Id, ad => ad.SellersPhotoUrl = e.SellersPhotoUrl);
                    break;
            }

            return Task.CompletedTask;
        }

        private void UpdateItem(Guid id, Action<ClassifiedAdDetails> update)
        {
            var item = _items.FirstOrDefault(x => x.ClassifiedAdId == id);
            if (item == null) return;

            update(item);
        }

        private void UpdateMultipleItems(
            Func<ClassifiedAdDetails, bool> query,
            Action<ClassifiedAdDetails> update
        )
        {
            foreach (var item in _items.Where(query))
                update(item);
        }
    }
}