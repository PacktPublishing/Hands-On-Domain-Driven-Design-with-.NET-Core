using System;
using System.Threading.Tasks;
using Marketplace.Projections;
using Raven.Client.Documents.Session;

namespace Marketplace.UserProfile
{
    public static class Queries
    {
        public static Task<ReadModels.UserDetails> GetUserDetails(
            this Func<IAsyncDocumentSession> getSession, Guid id)
        {
            using (var session = getSession())
                return session.LoadAsync<ReadModels.UserDetails>(id.ToString());
        }
    }
}