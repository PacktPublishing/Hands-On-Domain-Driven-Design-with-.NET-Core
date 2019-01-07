using System;

namespace Marketplace.Contracts
{
    public static class ClassifiedAds
    {
        public static class V1
        {
            /// <summary>
            /// Create a new ad command
            /// </summary>
            public class Create
            {
                /// <summary>
                /// New ad id
                /// </summary>
                public Guid Id { get; set; }

                /// <summary>
                /// Ad owner id
                /// </summary>
                public Guid OwnerId { get; set; }
            }
        }
    }
}