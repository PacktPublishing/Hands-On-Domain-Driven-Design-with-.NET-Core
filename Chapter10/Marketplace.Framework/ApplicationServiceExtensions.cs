using System;
using System.Threading.Tasks;

namespace Marketplace.Framework
{
    public static class ApplicationServiceExtensions
    {
        public static async Task HandleUpdate<T, TId>(this IApplicationService service,
            IAggregateStore store, TId aggregateId, Action<T> operation)
            where T : AggregateRoot<TId>
        {
            var aggregate = await store.Load<T, TId>(aggregateId);
            if (aggregate == null)
                throw new InvalidOperationException($"Entity with id {aggregateId.ToString()} cannot be found");

            operation(aggregate);
            await store.Save<T, TId>(aggregate);
        }
    }
}