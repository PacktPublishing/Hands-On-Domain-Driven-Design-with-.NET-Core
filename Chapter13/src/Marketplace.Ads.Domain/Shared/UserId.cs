using System;
using Marketplace.EventSourcing;

namespace Marketplace.Ads.Domain.Shared
{
    public class UserId : Value<UserId>
    {
        protected UserId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(
                    nameof(value),
                    "The Id cannot be empty"
                );

            Value = value;
        }

//        public static implicit operator UserId(string value)
//            => new UserId(Guid.Parse(value));

        public static UserId FromGuid(Guid value) => new UserId(value);

        public Guid Value { get; }

        public static implicit operator Guid(UserId self) => self.Value;

        public override string ToString() => Value.ToString();
    }
}