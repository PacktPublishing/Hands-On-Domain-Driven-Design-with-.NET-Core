using System;
using System.Threading.Tasks;
using Marketplace.RavenDb;
using Marketplace.Users.Messages.UserProfile;
using Raven.Client.Documents.Session;

namespace Marketplace.Users.Projections
{
    public static class UserDetailsProjection
    {
        public static Func<Task> GetHandler(
            IAsyncDocumentSession session,
            object @event)
        {
            Func<Guid, string> getDbId = ReadModels.UserDetails.GetDatabaseId;

            return @event switch
            {
                Events.V1.UserRegistered e => 
                    () => Create(e.UserId, e.DisplayName, e.FullName),
                Events.V1.UserDisplayNameUpdated e =>
                    () => Update(e.UserId, x => x.DisplayName = e.DisplayName),
                Events.V1.ProfilePhotoUploaded e =>
                    () => Update(e.UserId, x => x.PhotoUrl = e.PhotoUrl),
                _ => (Func<Task>) null
            };

            Task Update(Guid id, Action<ReadModels.UserDetails> update)
                => session.Update(getDbId(id), update);

            Task Create(Guid userId, string displayName, string fullName)
                => session.StoreAsync(
                    new ReadModels.UserDetails
                    {
                        Id = getDbId(userId),
                        DisplayName = displayName,
                        FullName = fullName
                    }
                );
        }
    }
}