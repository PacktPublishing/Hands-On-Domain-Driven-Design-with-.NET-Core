using System;
using System.Threading.Tasks;
using static Marketplace.Users.Projections.ReadModels;

namespace Marketplace.Users.UserProfiles
{
    public static class Queries
    {
        public static Task<UserDetails> GetUserDetails(
            this GetUsersModuleSession getSession,
            Guid id
        )
        {
            using var session = getSession();

            return session.LoadAsync<UserDetails>(
                UserDetails.GetDatabaseId(id)
            );
        }
    }
}