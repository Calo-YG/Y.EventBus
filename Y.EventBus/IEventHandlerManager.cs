namespace Y.EventBus
{
    public interface IEventHandlerManager
    {
         Task WriteAsync<TEto>(TEto eto) where TEto : class;

         Task CreateChannles();

         void Subscribe<TEto>() where TEto : class;
         Task Trigger();
    }
}
