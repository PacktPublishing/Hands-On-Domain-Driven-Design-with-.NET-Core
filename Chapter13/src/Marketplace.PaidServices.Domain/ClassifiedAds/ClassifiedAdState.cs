using System;
using System.Collections.Generic;
using Marketplace.EventSourcing;
using Marketplace.PaidServices.Domain.Orders;
using Marketplace.PaidServices.Messages.Ads;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public class ClassifiedAdState : AggregateState<ClassifiedAdState>
    {

        public override ClassifiedAdState When(
            ClassifiedAdState state,
            object @event
        )
            => With(
                @event switch
                {
                    Events.V1.Created e =>
                        With(state, x => x.Id = e.ClassifiedAdId)
                },
                x => x.Version++
            );

        protected override bool EnsureValidState(ClassifiedAdState newState)
            => true;

        List<ActivePaidService> ActiveServices { get; set; }
            = new List<ActivePaidService>();

        List<OrderId> OrderIds { get; set; }
            = new List<OrderId>();
    }
}