using System.Threading.Tasks;
using Marketplace.Ads.Domain.ClassifiedAds;
using Marketplace.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Marketplace.Ads.Messages.Ads.Commands;

namespace Marketplace.Ads.ClassifiedAds
{
    [ApiController, Route("api/ad"), Authorize]
    public class ClassifiedAdsCommandsApi : CommandApi<ClassifiedAd>
    {
        public ClassifiedAdsCommandsApi(
            ClassifiedAdsCommandService applicationService,
            ILoggerFactory loggerFactory)
            : base(applicationService, loggerFactory) { }

        [HttpPost]
        public Task<IActionResult> Post(V1.Create command)
            => HandleCommand(command, cmd => cmd.OwnerId = GetUserId());

        [Route("title"), HttpPut]
        public Task<IActionResult> Put(V1.ChangeTitle command)
            => HandleCommand(command);

        [Route("text"), HttpPut]
        public Task<IActionResult> Put(V1.UpdateText command)
            => HandleCommand(command);

        [Route("price"), HttpPut]
        public Task<IActionResult> Put(V1.UpdatePrice command)
            => HandleCommand(command);

        [Route("requestpublish"), HttpPut]
        public Task<IActionResult> Put(V1.RequestToPublish command)
            => HandleCommand(command);

        [Route("publish"), HttpPut]
        public Task<IActionResult> Put(V1.Publish command)
            => HandleCommand(command);

        [Route("delete"), HttpPost]
        public Task<IActionResult> Delete(V1.Delete command)
            => HandleCommand(command);

        [Route("image"), HttpPost]
        public Task<IActionResult> Post(V1.UploadImage command)
            => HandleCommand(command);
    }
}