using Microsoft.Extensions.DependencyInjection;

namespace Y.EventBus
{
    public class EventHandlerContainer : IEventHandlerContainer
    {
        public List<EventDiscription> Events { get; private set; }

        private readonly IServiceCollection Services;

        public EventHandlerContainer(IServiceCollection services)
        {
            Events = new List<EventDiscription>();
            Services = services;         
            services.AddSingleton<IEventHandlerContainer>(this);
        }

        private bool Check(Type type)
        {
            var discription = Events.FirstOrDefault(p=>p.EtoType == type);

            return discription is null;
        }
        
        ///订阅并且注入EventHandler
        public void TryAddChannle(Type eto,Type handler)
        {
            if(!Check(eto))
            {
                return;
            }

            Events.Add(new EventDiscription(eto, handler));

            var handlerbaseType = typeof(IEventHandler<>);

            var handlertype = handlerbaseType.MakeGenericType(eto);

            if(Services.Any(P=>P.ServiceType==handlertype))
            {
                return;
            }

            Services.AddTransient(handlertype, handler);
        }

        public void TryAddChannle<TEto, THandler>()
        {
            TryAddChannle(typeof(TEto),typeof(THandler));  
        }

        
        public void TryAddChannle(Type eto)
        {
            if (!Check(eto))
            {
                return;
            }

            Events.Add(new EventDiscription(eto));

            var handlerbaseType = typeof(IEventHandler<>);

            var handlertype = handlerbaseType.MakeGenericType(eto);

            if (Services.Any(P => P.ServiceType == handlertype))
            {
                return;
            }
        }

        public void TryAddChannle<TEto>()
        {
            TryAddChannle(typeof(TEto));
        }
    }
}
