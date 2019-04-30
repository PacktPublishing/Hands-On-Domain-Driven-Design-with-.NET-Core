using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using static Marketplace.Users.Projections.ReadModels;

namespace Marketplace.Users.UserProfiles
{
    [Route("api/profile")]
    public class UserProfileQueryApi : ControllerBase
    {
        readonly GetUsersModuleSession _getSession;

        public UserProfileQueryApi(
            GetUsersModuleSession getSession)
            => _getSession = getSession;

        [HttpGet("{userId}")]
        public async Task<ActionResult<UserDetails>> Get(Guid userId)
        {
            var user = await _getSession.GetUserDetails(userId);

            if (user == null) return NotFound();

            return user;
        }
    }
}