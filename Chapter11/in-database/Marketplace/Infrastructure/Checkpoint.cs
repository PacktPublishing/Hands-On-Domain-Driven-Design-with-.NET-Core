using EventStore.ClientAPI;

namespace Marketplace.Infrastructure
{
    public class Checkpoint
    {
        public string Id { get; set; }
        public Position Position { get; set; }
    }
}