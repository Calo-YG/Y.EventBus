using Microsoft.Extensions.DependencyInjection;
using Y.Module.DependencyInjection;
using Y.Module.Extensions;

namespace Y.EventBus
{
    public class EventHandlerContainer : IEventHandlerContainer,ISingletonInjection
    {
        public IReadOnlyList<EventDiscription> Events { get; private set; }

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
        public void TryAdd(Type eto,Type handler)
        {
            if(!Check(eto))
            {
                return;
            }

            Events.Append(new EventDiscription(eto, handler));

            var handlerbaseType = typeof(IEventHandler<>);

            var handlertype = handlerbaseType.MakeGenericType(eto);

            if(Services.IsExists(handlertype))
            {
                return;
            }

            Services.AddTransient(handlertype, handler);
        }

        public void TryAdd<TEto, THandler>()
        {
            TryAdd(typeof(TEto),typeof(THandler));  
        }

        
        public void TryAdd(Type eto)
        {
            if (!Check(eto))
            {
                return;
            }

            Events.Append(new EventDiscription(eto, null));

            var handlerbaseType = typeof(IEventHandler<>);

            var handlertype = handlerbaseType.MakeGenericType(eto);

            if (Services.IsExists(handlertype))
            {
                return;
            }
        }

        public void TryAdd<TEto>()
        {
            TryAdd(typeof(TEto));
        }
    }
}
