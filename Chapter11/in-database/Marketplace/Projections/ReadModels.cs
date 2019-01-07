using System;

namespace Marketplace.Projections
{
    public static class ReadModels
    {
        public class ClassifiedAdDetails
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public decimal Price { get; set; }
            public string CurrencyCode { get; set; }
            public string Description { get; set; }
            public Guid SellerId { get; set; }
            public string SellersDisplayName { get; set; }
            public string SellersPhotoUrl { get; set; }
            public string[] PhotoUrls { get; set; }
        }

        public class UserDetails
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string PhotoUrl { get; set; }
        }
    }
}