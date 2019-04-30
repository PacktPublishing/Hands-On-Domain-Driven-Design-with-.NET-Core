using static Marketplace.Ads.Messages.Ads.Events;
using static Marketplace.EventSourcing.TypeMapper;

namespace Marketplace.Ads
{
    public static class EventMappings
    {
        public static void MapEventTypes()
        {
            Map<V1.ClassifiedAdCreated>("ClassifiedAdCreated");
            Map<V1.ClassifiedAdDeleted>("ClassifiedAdDeleted");
            Map<V1.ClassifiedAdPublished>("ClassifiedAdPublished");
            Map<V1.ClassifiedAdTextUpdated>("ClassifiedAdTextUpdated");
            Map<V1.ClassifiedAdPriceUpdated>("ClassifiedAdPriceUpdated");
            Map<V1.ClassifiedAdTitleChanged>("ClassifiedAdTitleChanged");
            Map<V1.ClassifiedAdPictureResized>("ClassifiedAdPictureResized");
            Map<V1.ClassifiedAdSentForReview>("ClassifiedAdSentForReview");
            Map<V1.PictureAddedToAClassifiedAd>("PictureAddedToAClassifiedAd");
        }
    }
}