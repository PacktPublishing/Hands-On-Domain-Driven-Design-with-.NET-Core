using System;
using System.Threading.Tasks;
using Marketplace.Domain.UserProfile;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;

namespace Marketplace.Projections
{
    public class UserDetailsProjection : RavenDbProjection<ReadModels.UserDetails>
    {
        public UserDetailsProjection(Func<IAsyncDocumentSession> getSession)
            : base(getSession)
        {
        }

        public override async Task Project(object @event)
        {
            switch (@event)
            {
                case Events.UserRegistered e:
                    await UsingSession(session =>
                        session.StoreAsync(new ReadModels.UserDetails
                        {
                            Id = e.UserId.ToString(),
                            DisplayName = e.DisplayName
                        }));
                    break;
                case Events.UserDisplayNameUpdated e:
                    await UsingSession(session =>
                        UpdateItem(session, e.UserId, x => x.DisplayName = e.DisplayName));
                    break;
                case Events.ProfilePhotoUploaded e:
                    await UsingSession(session =>
                        UpdateItem(session, e.UserId, x => x.PhotoUrl = e.PhotoUrl));
                    break;
            }
        }
    }
}