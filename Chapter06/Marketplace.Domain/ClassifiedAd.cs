using Marketplace.Framework;

namespace Marketplace.Domain
{
    public class ClassifiedAd : Entity<ClassifiedAdId>
    {
        public ClassifiedAd(ClassifiedAdId id, UserId ownerId) =>
            Apply(new Events.ClassifiedAdCreated
            {
                Id = id,
                OwnerId = ownerId
            });

        public void SetTitle(ClassifiedAdTitle title) =>
            Apply(new Events.ClassifiedAdTitleChanged
            {
                Id = Id,
                Title = title
            });

        public void UpdateText(ClassifiedAdText text) =>
            Apply(new Events.ClassifiedAdTextUpdated
            {
                Id = Id,
                AdText = text
            });

        public void UpdatePrice(Price price) =>
            Apply(new Events.ClassifiedAdPriceUpdated
            {
                Id = Id,
                Price = price.Amount,
                CurrencyCode = price.Currency.CurrencyCode
            });

        public void RequestToPublish()
        {
            if (Title == null)
                throw new InvalidEntityStateException(this, "title cannot be empty");

            if (Text == null)
                throw new InvalidEntityStateException(this, "text cannot be empty");

            if (Price?.Amount == 0)
                throw new InvalidEntityStateException(this, "price cannot be zero");
            
            Apply(new Events.ClassidiedAdSentForReview {Id = Id});
        }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.ClassifiedAdCreated e:
                    Id = new ClassifiedAdId(e.Id);
                    OwnerId = new UserId(e.OwnerId);
                    State = ClassifiedAdState.Inactive;
                    break;
                case Events.ClassifiedAdTitleChanged e:
                    Title = new ClassifiedAdTitle(e.Title);
                    break;
                case Events.ClassifiedAdTextUpdated e:
                    Text = new ClassifiedAdText(e.AdText);
                    break;
                case Events.ClassifiedAdPriceUpdated e:
                    Price = new Price(e.Price, e.CurrencyCode);
                    break;
                case Events.ClassidiedAdSentForReview e:
                    State = ClassifiedAdState.PendingReview;
                    break;
            }
        }

        public ClassifiedAdId Id { get; private set; }
        public UserId OwnerId { get; private set; }
        public ClassifiedAdTitle Title { get; private set; }
        public ClassifiedAdText Text { get; private set; }
        public Price Price { get; private set; }
        public ClassifiedAdState State { get; private set; }
        public UserId ApprovedBy { get; private set; }

        public enum ClassifiedAdState
        {
            PendingReview,
            Active,
            Inactive,
            MarkedAsSold
        }
    }
}