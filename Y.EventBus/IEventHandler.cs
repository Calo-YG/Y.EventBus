namespace Y.EventBus
{
    public interface IEventHandler<TEto>  where TEto : class
    {
        Task HandelrAsync(TEto eto);
    }
}
