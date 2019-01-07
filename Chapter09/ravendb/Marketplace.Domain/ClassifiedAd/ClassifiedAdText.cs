using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class ClassifiedAdText : Value<ClassifiedAdText>
    {
        public string Value { get; private set; }

        internal ClassifiedAdText(string text) => Value = text;
        
        public static ClassifiedAdText FromString(string text) =>
            new ClassifiedAdText(text);
        
        public static implicit operator string(ClassifiedAdText text) =>
            text.Value;
        
        // Satisfy the serialization requirements 
        protected ClassifiedAdText() { }
    }
}