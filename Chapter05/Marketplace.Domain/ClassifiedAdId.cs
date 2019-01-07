using System;

namespace Marketplace.Domain
{
    public class ClassifiedAdId : IEquatable<ClassifiedAdId>
    {
        private Guid Value { get; }

        public ClassifiedAdId(Guid value)
        {
            if (value == default)
                throw new ArgumentNullException(nameof(value), "Classified Ad id cannot be empty");
            
            Value = value;
        }

        public static implicit operator Guid(ClassifiedAdId self) => self.Value;
        
        public static implicit operator ClassifiedAdId(string value) 
            => new ClassifiedAdId(Guid.Parse(value));

        public override string ToString() => Value.ToString();

        public bool Equals(ClassifiedAdId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClassifiedAdId) obj);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }
    }
}