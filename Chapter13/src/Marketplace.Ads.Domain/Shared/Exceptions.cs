using System;

namespace Marketplace.Ads.Domain.Shared
{
    public static class DomainExceptions
    {
        public class ProfanityFound : Exception
        {
            public ProfanityFound(string text)
                : base($"Profanity found in text: {text}") { }
        }

        public class InvalidEntityState : Exception
        {
            public InvalidEntityState(object entity, string message)
                : base(
                    $"Entity {entity.GetType().Name} state change rejected, {message}"
                ) { }
        }
    }
}