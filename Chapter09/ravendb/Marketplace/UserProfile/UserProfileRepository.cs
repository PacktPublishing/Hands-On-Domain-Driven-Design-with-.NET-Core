using Marketplace.Domain.Shared;
using Marketplace.Domain.UserProfile;
using Marketplace.Infrastructure;
using Raven.Client.Documents.Session;

namespace Marketplace.UserProfile
{
    public class UserProfileRepository
        : RavenDbRepository<Domain.UserProfile.UserProfile, UserId>, IUserProfileRepository
    {
        public UserProfileRepository(IAsyncDocumentSession session) 
            : base(session, id => $"UserProfile/{id.Value.ToString()}") { }
    }
}