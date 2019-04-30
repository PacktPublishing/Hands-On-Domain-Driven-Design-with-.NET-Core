using System;
using Marketplace.EventSourcing;

namespace Marketplace.PaidServices.Domain.Shared
{
    public class UserId : Value<UserId>
    {
        UserId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(
                    nameof(value), 
                    "The Id cannot be empty");
            
            Value = value;
        }
        
        public static UserId FromGuid(Guid value) => new UserId(value);

        Guid Value { get; }
        
        public static implicit operator Guid(UserId self) => self.Value;

        public override string ToString() => Value.ToString();
    }
}