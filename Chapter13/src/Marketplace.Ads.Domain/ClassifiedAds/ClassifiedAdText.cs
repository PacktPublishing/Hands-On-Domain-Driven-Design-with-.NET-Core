using Marketplace.EventSourcing;

namespace Marketplace.Ads.Domain.ClassifiedAds
{
    public class ClassifiedAdText : Value<ClassifiedAdText>
    {
        internal ClassifiedAdText(string text) => Value = text;

        // Satisfy the serialization requirements 
        protected ClassifiedAdText() { }
        public string Value { get; }

        public static ClassifiedAdText FromString(string text) => new ClassifiedAdText(text);

        public static implicit operator string(ClassifiedAdText text) => text.Value;
    }
}