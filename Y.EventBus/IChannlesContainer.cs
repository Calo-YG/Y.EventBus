namespace Y.EventBus
{
    public interface IEventHandlerContainer
    {
        public IReadOnlyList<EventDiscription> Events { get; }
    }
}
