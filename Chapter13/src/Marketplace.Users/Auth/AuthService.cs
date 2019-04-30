using System;
using System.Threading.Tasks;
using Marketplace.Users.UserProfiles;

namespace Marketplace.Users.Auth
{
    public class AuthService
    {
        readonly GetUsersModuleSession _getSession;

        public AuthService(GetUsersModuleSession getSession)
            => _getSession = getSession;

        public async Task<bool> CheckCredentials(
            string userName,
            string password
        )
        {
            var userDetails =
                await _getSession.GetUserDetails(Guid.Parse(password));

            return userDetails != null && userDetails.DisplayName == userName;
        }
    }
}