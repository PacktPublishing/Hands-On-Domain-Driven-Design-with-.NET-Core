using System;
using Marketplace.Domain.Shared;
using Marketplace.Framework;

namespace Marketplace.Domain.UserProfile
{
    public class DisplayName : Value<DisplayName>
    {
        public string Value { get; private set; }

        internal DisplayName(string displayName) => Value = displayName;

        public static DisplayName FromString(
            string displayName,
            CheckTextForProfanity hasProfanity)
        {
            if (displayName.IsEmpty())
                throw new ArgumentNullException(nameof(FullName));
            
            if (hasProfanity(displayName).GetAwaiter().GetResult())
                throw new DomainExceptions.ProfanityFound(displayName);

            return new DisplayName(displayName);
        }

        public static implicit operator string(DisplayName displayName)
            => displayName.Value;
        
        // Satisfy the serialization requirements
        protected DisplayName() { }
    }
}