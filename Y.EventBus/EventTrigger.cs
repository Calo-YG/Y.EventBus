namespace Y.EventBus
{
    public class EventTrigger
    {
        public event Func<Task> Event;

        public EventTrigger()
        {

        }

        public void Recived(Func<Task> func)
        {
            if (Event is not null)
            {
                return;
            }
            Event += func;
        }

        public Task Trigger()
        {
            return Event();
        }
    }
}
