using Y.Module.DependencyInjection;

namespace Y.EventBus
{
    public class LocalEventBus : ILocalEventBus,ITransientInjection
    {
        private readonly IEventHandlerManager _eventHandlerManager;
        public LocalEventBus(IEventHandlerManager eventHandlerManager)
        {
            _eventHandlerManager = eventHandlerManager;
        }
        public async Task PublichAsync<TEto>(TEto eto, CancellationToken cancellationToken) where TEto : class
        {
            await _eventHandlerManager.WriteAsync(eto);
        }
    }
}
