using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Marketplace.Api
{
    [Route("/ad")]
    public class ClassifiedAdsCommandsApi : Controller
    {
        private readonly ClassifiedAdsApplicationService _applicationService;

        public ClassifiedAdsCommandsApi(
            ClassifiedAdsApplicationService applicationService)
            => _applicationService = applicationService;

        [HttpPost]
        public async Task<IActionResult> Post(
            Contracts.ClassifiedAds.V1.Create request)
        {
            await _applicationService.Handle(request);

            Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            
            return Ok();
        }
    }
}