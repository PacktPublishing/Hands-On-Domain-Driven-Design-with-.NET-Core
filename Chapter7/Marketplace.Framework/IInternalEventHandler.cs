namespace Marketplace.Framework
{
    public interface IInternalEventHandler
    {
        void Handle(object @event);
    }
}