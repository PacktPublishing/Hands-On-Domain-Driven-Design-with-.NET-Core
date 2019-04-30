using System;
using Marketplace.EventSourcing;

namespace Marketplace.PaidServices.Domain.ClassifiedAds
{
    public class ClassifiedAdId : Value<ClassifiedAdId>
    {
        ClassifiedAdId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(
                    nameof(value),
                    "The Id cannot be empty"
                );

            Value = value;
        }

        Guid Value { get; }

        public static ClassifiedAdId FromGuid(Guid value)
            => new ClassifiedAdId(value);

        public static implicit operator Guid(ClassifiedAdId self) => self.Value;

        public override string ToString() => Value.ToString();
    }
}