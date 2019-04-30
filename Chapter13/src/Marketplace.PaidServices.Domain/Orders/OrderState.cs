using System;
using System.Collections.Generic;
using System.Linq;
using Marketplace.EventSourcing;
using Marketplace.PaidServices.Domain.Services;
using static Marketplace.PaidServices.Messages.Orders.Events;

namespace Marketplace.PaidServices.Domain.Orders
{
    public class OrderState : AggregateState<OrderState>
    {
        public override OrderState When(
            OrderState state,
            object @event
        )
            => With(
                @event switch 
                {
                    V1.OrderCreated e =>
                        With(state, x => x.AdId = e.ClassifiedAdId),
                    V1.ServiceAddedToOrder e =>
                        With(
                            state, x => x.Services.Add(
                                PaidService.Find(e.ServiceType)
                            )
                        ),
                    V1.ServiceRemovedFromOrder e =>
                        With(
                            state, x => x.Services.RemoveAll(
                                s => s.Type ==
                                     PaidService.ParseString(e.ServiceType)
                            )
                        ),
                    V1.OrderFulfilled _ =>
                        With(state, x => x.Status = OrderStatus.Completed),
                    _ => this
                }, 
                x => x.Version++
            );

        protected override bool EnsureValidState(OrderState newState)
            => newState switch 
                {
                    { } o when o.AdId == Guid.Empty => false, 
                    { } o when o.Status == OrderStatus.Completed
                               && o.Services.Count != Services.Count 
                               => false,
                    _ => true
                };

        internal double GetTotal() => Services.Sum(x => x.Price);

        internal Guid AdId { get; set; }
        OrderStatus Status { get; set; } = OrderStatus.New;
        internal List<PaidService> Services { get; } = new List<PaidService>();

        internal string[] GetServices()
            => Services.Select(x => x.Type.ToString()).ToArray();

        enum OrderStatus { New, Completed }
    }
}