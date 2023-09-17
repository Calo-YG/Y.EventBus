namespace Y.EventBus
{
    public interface ILocalEventBus
    {
        public Task PublichAsync<TEto>(TEto eto) where TEto: class;
    }
}
