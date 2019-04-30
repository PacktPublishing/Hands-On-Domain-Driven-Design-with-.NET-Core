namespace Marketplace.EventSourcing
{
    public interface IInternalEventHandler
    {
        void Handle(object @event);
    }
}