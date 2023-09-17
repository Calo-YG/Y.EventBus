namespace Y.EventBus
{
    public class LocalEventBus : ILocalEventBus
    {
        private readonly IEventHandlerManager _eventHandlerManager;
        public LocalEventBus(IEventHandlerManager eventHandlerManager)
        {
            _eventHandlerManager = eventHandlerManager;
        }
        public async Task PublichAsync<TEto>(TEto eto) where TEto : class
        {
            await _eventHandlerManager.WriteAsync(eto);
        }
    }
}
