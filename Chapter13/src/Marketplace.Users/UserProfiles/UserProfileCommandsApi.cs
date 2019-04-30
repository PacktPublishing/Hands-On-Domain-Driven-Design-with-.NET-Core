using System.Threading.Tasks;
using Marketplace.Users.Domain.UserProfiles;
using Marketplace.WebApi;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Marketplace.Users.Messages.UserProfile.Commands;

namespace Marketplace.Users.UserProfiles
{
    [Route("api/profile")]
    public class UserProfileCommandsApi
        : CommandApi<UserProfile>
    {
        public UserProfileCommandsApi(
            UserProfileCommandService applicationService,
            ILoggerFactory loggerFactory)
            : base(applicationService, loggerFactory) { }

        [HttpPost]
        public Task<IActionResult> Post([FromBody] V1.RegisterUser request)
            => HandleCommand(request);

        [Route("fullname"), HttpPut]
        public Task<IActionResult> Put(V1.UpdateUserFullName request)
            => HandleCommand(request);

        [Route("displayname"), HttpPut]
        public Task<IActionResult> Put(
            V1.UpdateUserDisplayName request)
            => HandleCommand(request);

        [Route("photo"), HttpPut]
        public Task<IActionResult> Put(
            V1.UpdateUserProfilePhoto request)
            => HandleCommand(request);
    }
}