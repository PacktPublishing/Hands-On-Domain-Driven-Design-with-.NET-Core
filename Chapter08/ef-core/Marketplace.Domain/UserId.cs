using System;

namespace Marketplace.Domain
{
    public class UserId
    {
        protected UserId() {}
        
        public Guid Value { get; internal set; }

        public UserId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "User id cannot be empty");
            
            Value = value;
        }
        
        public static implicit operator Guid(UserId self) => self.Value;
        
        public static UserId NoUser =>
            new UserId();
    }
}