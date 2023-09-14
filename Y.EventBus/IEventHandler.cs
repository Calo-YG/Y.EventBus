namespace Y.EventBus
{
    public interface IEventHandler<TEto> :IEventHandler where TEto : class
    {
    }

    public interface IEventHandler
    {
        public Task HandelrAsync(string eto);
    }
}
