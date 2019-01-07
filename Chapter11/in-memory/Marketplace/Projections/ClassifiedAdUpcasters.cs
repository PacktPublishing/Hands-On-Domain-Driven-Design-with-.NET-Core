using System;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Marketplace.Framework;
using Marketplace.Infrastructure;

namespace Marketplace.Projections
{
    public class ClassifiedAdUpcasters : IProjection
    {
        private readonly IEventStoreConnection _eventStoreConnection;
        private readonly Func<Guid, string> _getUserPhoto;
        private const string StreamName = "UpcastedClassifiedAdEvents";

        public ClassifiedAdUpcasters(IEventStoreConnection eventStoreConnection,
            Func<Guid, string> getUserPhoto)
        {
            _eventStoreConnection = eventStoreConnection;
            _getUserPhoto = getUserPhoto;
        }

        public async Task Project(object @event)
        {
            switch (@event)
            {
                case Domain.ClassifiedAd.Events.ClassifiedAdPublished e:
                    var photoUrl = _getUserPhoto(e.OwnerId);
                    var newEvent = new ClassifiedAdUpcastedEvents.V1.ClassifiedAdPublished
                    {
                        Id = e.Id,
                        OwnerId = e.OwnerId,
                        ApprovedBy = e.ApprovedBy,
                        SellersPhotoUrl = photoUrl
                    };
                    await _eventStoreConnection.AppendEvents(
                        StreamName,
                        ExpectedVersion.Any,
                        newEvent);
                    break;
            }
        }
    }

    public static class ClassifiedAdUpcastedEvents
    {
        public static class V1
        {
            public class ClassifiedAdPublished
            {
                public Guid Id { get; set; }
                public Guid OwnerId { get; set; }
                public string SellersPhotoUrl { get; set; }
                public Guid ApprovedBy { get; set; }
            }
        }
    }
}