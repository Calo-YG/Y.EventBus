using System.Collections.Concurrent;
namespace Y.EventBus
{
    public interface IEventHandlerContainer
    {
        public ConcurrentBag<EventDiscription> Events { get; }
    }
}
