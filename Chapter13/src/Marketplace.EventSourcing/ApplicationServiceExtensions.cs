using System;
using System.Threading.Tasks;

namespace Marketplace.EventSourcing
{
    public static class ApplicationServiceExtensions
    {
        public static async Task HandleUpdate<T>(
            this IApplicationService service,
            IAggregateStore store,
            AggregateId<T> aggregateId,
            Action<T> operation)
            where T : AggregateRoot
        {
            var aggregate = await store.Load(aggregateId);

            if (aggregate == null)
                throw new InvalidOperationException($"Entity with id {aggregateId} cannot be found");

            operation(aggregate);
            await store.Save(aggregate);
        }
    }
}