using System;

namespace Marketplace.PaidServices.Domain
{
    public static class DomainExceptions
    {
        public class OperationNotAllowed : Exception
        {
            public OperationNotAllowed(
                object entity,
                string operation,
                string state
            ) : base(
                $"Operation {operation} is not allowed for the " +
                $"entity {entity.GetType().Name} in state {state}"
            ) { }
        }
    }
}