using System.Linq;
using Marketplace.PaidServices.Domain.ClassifiedAds;
using Marketplace.PaidServices.Domain.Services;
using Marketplace.PaidServices.Domain.Shared;
using static Marketplace.PaidServices.Messages.Orders.Events;

namespace Marketplace.PaidServices.Domain.Orders
{
    public static class Order
    {
        public static OrderState.Result Create(
            OrderId orderId,
            ClassifiedAdId classifiedAdId,
            UserId customerId
        )
            => new OrderState().Apply(
                new V1.OrderCreated
                {
                    OrderId = orderId,
                    ClassifiedAdId = classifiedAdId,
                    CustomerId = customerId
                }
            );

        public static OrderState.Result AddService(
            OrderState state,
            PaidService service
        )
            => state.Services.Contains(service)
                ? state.NoEvents()
                : state.Apply(
                    new V1.ServiceAddedToOrder
                    {
                        OrderId = state.Id,
                        ServiceType = service.Type.ToString(),
                        Description = service.Description,
                        Price = service.Price
                    },
                    new V1.OrderTotalUpdated
                    {
                        OrderId = state.Id,
                        Total = state.GetTotal() + service.Price
                    }
                );

        public static OrderState.Result RemoveService(
            OrderState state,
            PaidService service
        )
            => !state.Services.Contains(service)
                ? state.NoEvents()
                : state.Apply(
                    new V1.ServiceRemovedFromOrder
                    {
                        OrderId = state.Id,
                        ServiceType = service.Type.ToString()
                    },
                    new V1.OrderTotalUpdated
                    {
                        OrderId = state.Id,
                        Total = state.GetTotal() - service.Price
                    }
                );

        public static OrderState.Result Fulfill(OrderState state)
            => state.Services.Any()
                ? state.Apply(
                    new V1.OrderFulfilled
                    {
                        OrderId = state.Id,
                        ClassifiedAdId = state.AdId,
                        Services = state.Services
                            .Select(
                                x => new V1.OrderFulfilled.Service
                                {
                                    Description = x.Description,
                                    Price = x.Price,
                                    Type = x.Type.ToString()
                                }
                            )
                            .ToArray()
                    }
                )
                : state.Apply(
                    new V1.OrderDeleted
                    {
                        OrderId = state.Id
                    }
                );
    }
}