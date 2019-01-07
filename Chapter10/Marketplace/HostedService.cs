using System.Threading;
using System.Threading.Tasks;
using EventStore.ClientAPI;
using Microsoft.Extensions.Hosting;

namespace Marketplace
{
    public class HostedService : IHostedService
    {
        private readonly IEventStoreConnection _esConnection;

        public HostedService(IEventStoreConnection esConnection)
        {
            _esConnection = esConnection;
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => _esConnection.ConnectAsync();

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _esConnection.Close();
            return Task.CompletedTask;
        }
    }
}