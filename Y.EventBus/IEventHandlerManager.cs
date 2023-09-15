namespace Y.EventBus
{
    public interface IEventHandlerManager
    {
         Task WriteAsync<TEto>(TEto eto) where TEto : class;

         Task Subcrice();

         Task Comsuer<TEto>() where TEto : class;
    }
}
