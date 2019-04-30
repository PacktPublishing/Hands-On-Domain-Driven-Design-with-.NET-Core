using System;

namespace Marketplace.Users.Projections
{
    public static class ReadModels
    {
        public class UserDetails
        {
            public string Id { get; set; }
            public string DisplayName { get; set; }
            public string FullName { get; set; }
            public string PhotoUrl { get; set; }

            public static string GetDatabaseId(Guid id) => $"UserDetails/{id}";
        }
    }
}