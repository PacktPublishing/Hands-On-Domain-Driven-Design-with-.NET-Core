using System;
using Marketplace.EventSourcing;

namespace Marketplace.Ads.Domain.ClassifiedAds
{
    public class ClassifiedAdId : AggregateId<ClassifiedAd>
    {
        ClassifiedAdId(Guid value) : base(value) { }
        
        public static ClassifiedAdId FromGuid(Guid value)
            => new ClassifiedAdId(value);
    }
}