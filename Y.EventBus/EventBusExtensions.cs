using Microsoft.Extensions.DependencyInjection;
using Y.Module.Extensions;

namespace Y.EventBus
{
    public  static class EventBusExtensions
    {
        public static IServiceCollection EvenrBusSubcrice(this IServiceCollection services,Action<EventHandlerContainer> action)
        {
            services.ChcekNull();

            EventHandlerContainer eventHandlerContainer = new EventHandlerContainer(services);

            action.Invoke(eventHandlerContainer);

            return services;
        }
    }
}
