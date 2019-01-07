using System.Collections.Generic;
using Marketplace.Infrastructure;
using Marketplace.Projections;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Marketplace.ClassifiedAd
{
    [Route("/ad")]
    public class ClassifiedAdsQueryApi : Controller
    {
        private static ILogger _log = Log.ForContext<ClassifiedAdsQueryApi>();
        
        private readonly IEnumerable<ReadModels.ClassifiedAdDetails> _items;

        public ClassifiedAdsQueryApi(IEnumerable<ReadModels.ClassifiedAdDetails> items) 
            => _items = items;

        [HttpGet]
        public IActionResult Get(QueryModels.GetPublicClassifiedAd request)
            => RequestHandler.HandleQuery(() => _items.Query(request), _log);
    }
}