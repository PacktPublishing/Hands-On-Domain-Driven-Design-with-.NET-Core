using System;
using Marketplace.Framework;

namespace Marketplace.Domain.ClassifiedAd
{
    public class Picture : Entity<PictureId>
    {
        public ClassifiedAdId ParentId { get; private set; }
        public PictureSize Size { get; private set; }
        public Uri Location { get; private set; }
        public int Order { get; private set; }

        protected override void When(object @event)
        {
            switch (@event)
            {
                case Events.PictureAddedToAClassifiedAd e:
                    ParentId = new ClassifiedAdId(e.ClassifiedAdId);
                    Id = new PictureId(e.PictureId);
                    Location = new Uri(e.Url);
                    Size = new PictureSize {Height = e.Height, Width = e.Width};
                    Order = e.Order;
                    break;
                case Events.ClassifiedAdPictureResized e:
                    Size = new PictureSize{Height = e.Height, Width = e.Width};
                    break;
            }
        }
        
        public void Resize(PictureSize newSize)
            => Apply(new Events.ClassifiedAdPictureResized
            {
                PictureId = Id.Value,
                ClassifiedAdId = ParentId.Value,
                Height = newSize.Width,
                Width = newSize.Width
            });

        public Picture(Action<object> applier) : base(applier)
        {
        }
    }

    public class PictureId : Value<PictureId>
    {
        public PictureId(Guid value) => Value = value;

        public Guid Value { get; }
    }
}