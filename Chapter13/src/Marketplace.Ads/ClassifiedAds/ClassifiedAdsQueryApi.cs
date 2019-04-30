using System;
using System.Threading.Tasks;
using Marketplace.Ads.Projections;
using Marketplace.Modules.ClassifiedAds;
using Marketplace.RavenDb;
using Microsoft.AspNetCore.Mvc;
using Raven.Client.Documents.Session;

namespace Marketplace.Ads.ClassifiedAds
{
    [ApiController, Route("/ad")]
    public class ClassifiedAdsQueryApi : ControllerBase
    {
        readonly Func<IAsyncDocumentSession> _getSession;

        public ClassifiedAdsQueryApi(Func<IAsyncDocumentSession> getSession) 
            => _getSession = getSession;

        [HttpGet]
        public Task<ActionResult<ReadModels.ClassifiedAdDetails>> Get(
            [FromQuery] QueryModels.GetPublicClassifiedAd request)
            => _getSession.RunApiQuery(s => s.Query(request));
    }
}