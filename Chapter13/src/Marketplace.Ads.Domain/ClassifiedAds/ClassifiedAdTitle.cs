using System;
using System.Text.RegularExpressions;
using Marketplace.EventSourcing;
using static System.String;

namespace Marketplace.Ads.Domain.ClassifiedAds
{
    public class ClassifiedAdTitle : Value<ClassifiedAdTitle>
    {
        internal ClassifiedAdTitle(string value) => Value = value;

        // Satisfy the serialization requirements 
        protected ClassifiedAdTitle() { }

        public string Value { get; }

        public static ClassifiedAdTitle FromString(string title)
        {
            CheckValidity(title);
            return new ClassifiedAdTitle(title);
        }

        public static ClassifiedAdTitle FromHtml(string htmlTitle)
        {
            var supportedTagsReplaced = htmlTitle
                .Replace("<i>", "*")
                .Replace("</i>", "*")
                .Replace("<b>", "**")
                .Replace("</b>", "**");

            var value = Regex.Replace(supportedTagsReplaced, "<.*?>", Empty);
            CheckValidity(value);

            return new ClassifiedAdTitle(value);
        }

        public static implicit operator string(ClassifiedAdTitle title) => title.Value;

        static void CheckValidity(string value)
        {
            if (IsNullOrEmpty(value))
                throw new ArgumentNullException(
                    nameof(value),
                    "Title cannot be empty");
            
            if (value.Length < 10)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Title cannot be shorter than 10 characters");
            
            if (value.Length > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Title cannot be longer than 100 characters");
        }
    }
}