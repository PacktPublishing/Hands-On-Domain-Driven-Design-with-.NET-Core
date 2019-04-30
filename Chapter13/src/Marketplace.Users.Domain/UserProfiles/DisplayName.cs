using System;
using Marketplace.EventSourcing;
using Marketplace.Users.Domain.Shared;

namespace Marketplace.Users.Domain.UserProfiles
{
    public class DisplayName : Value<DisplayName>
    {
        internal DisplayName(string displayName) => Value = displayName;

        // Satisfy the serialization requirements
        protected DisplayName() { }
        public string Value { get; }

        public static DisplayName FromString(
            string displayName,
            CheckTextForProfanity hasProfanity
        )
        {
            if (displayName.IsEmpty())
                throw new ArgumentNullException(nameof(FullName));

            if (hasProfanity(displayName).GetAwaiter().GetResult())
                throw new DomainExceptions.ProfanityFound(displayName);

            return new DisplayName(displayName);
        }

        public static implicit operator string(DisplayName displayName)
            => displayName.Value;
    }
}