using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.PaidServices.Domain.Orders;
using Marketplace.PaidServices.Domain.Services;
using Marketplace.PaidServices.Domain.Shared;
using Marketplace.PaidServices.Messages.Orders;
using static Marketplace.PaidServices.Messages.Ads.Events;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public static class ClassifiedAd
    {
        public static ClassifiedAdState.Result Create(
            ClassifiedAdId id,
            UserId sellerId
        )
            => new ClassifiedAdState().Apply(
                new V1.Created
                {
                    ClassifiedAdId = id,
                    SellerId = sellerId
                }
            );

        public static ClassifiedAdState.Result FulfillOrder(
            ClassifiedAdState state,
            DateTimeOffset when,
            IEnumerable<PaidService> services
        )
            => state.Apply(
                services.Select(
                    x => new V1.ServiceActivated
                    {
                        ClassifiedAdId = state.Id,
                        ServiceType = x.Type.ToString(),
                        ActiveUntil = when + x.Duration
                    }
                )
            );
    }
}