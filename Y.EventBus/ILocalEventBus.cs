namespace Y.EventBus
{
    public interface ILocalEventBus
    {
        public Task PublichAsync<TEto>(TEto eto, CancellationToken cancellationToken) where TEto: class;
    }
}
