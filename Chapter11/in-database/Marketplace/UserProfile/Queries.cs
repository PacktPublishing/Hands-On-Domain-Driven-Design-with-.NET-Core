using System;
using System.Threading.Tasks;
using Raven.Client.Documents.Session;
using static Marketplace.Projections.ReadModels;

namespace Marketplace.UserProfile
{
    public static class Queries
    {
        public static Task<UserDetails> GetUserDetails(
            this Func<IAsyncDocumentSession> getSession,
            Guid id
        )
        {
            using var session = getSession();

            return session.LoadAsync<UserDetails>(id.ToString());
        }
    }
}