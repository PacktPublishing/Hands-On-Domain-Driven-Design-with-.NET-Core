using Raven.Client.Documents;
using Raven.Client.ServerWide;
using Raven.Client.ServerWide.Operations;

namespace Marketplace.Infrastructure.RavenDb
{
    public static class Configuration
    {
        public static IDocumentStore ConfigureRavenDb(
            string serverUrl
        )
        {
            var store = new DocumentStore
            {
                Urls = new[] {serverUrl}
            };
            store.Initialize();

            return store;
        }
    }
}