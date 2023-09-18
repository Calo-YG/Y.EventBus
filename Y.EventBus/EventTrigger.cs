namespace Y.EventBus
{
    public class EventTrigger:IDisposable
    {
        public event Func<Task>? Event;

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
            if(Event is null)
            {
                return Task.CompletedTask;  
            }
            return Event();
        }

        public void Dispose()
        {
            if( Event is not null )
            {
                Event = null;
            }
        }
    }
}
