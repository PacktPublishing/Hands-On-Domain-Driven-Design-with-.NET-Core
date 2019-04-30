using System;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.EventStore;
using Marketplace.PaidServices.ClassifiedAds;
using Events = Marketplace.PaidServices.Messages.Orders.Events;
using V1 = Marketplace.PaidServices.Messages.Ads.Commands.V1;

namespace Marketplace.PaidServices.Reactors
{
    public class OrderReactor : ReactorBase
    {
        public OrderReactor(
            ClassifiedAdCommandService service
        ) : base(@event => React(service, @event)) { }

        static Func<Task> React(
            ClassifiedAdCommandService service,
            object @event
        )
            => @event switch
            {
                Events.V1.OrderCreated e =>
                    () => service.Handle(
                        new V1.Create
                        {
                            ClassifiedAdId = e.ClassifiedAdId,
                            SellerId = e.CustomerId
                        }
                    ),
                Events.V1.OrderFulfilled e =>
                    () => service.Handle(
                        new V1.FulfillOrder
                        {
                            ClassifiedAdId = e.ClassifiedAdId,
                            When = e.FulfilledAt,
                            ServiceTypes = e.Services
                                .Select(x => x.Type)
                                .ToArray()
                        }
                    ),
                _ => (Func<Task>) null
            };
    }
}