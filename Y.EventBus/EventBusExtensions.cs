using Microsoft.Extensions.DependencyInjection;
using Y.Module.Extensions;

namespace Y.EventBus
{
    public  static class EventBusExtensions
    {
        public static IServiceCollection AddEventBusWithHandeler(this IServiceCollection services,Action<EventHandlerContainer> action)
        {
            services.ChcekNull();

            services.AddSingleton<IEventHandlerManager, EventHandlerManager>();

            services.AddSingleton<ILocalEventBus, LocalEventBus>();

            EventHandlerContainer eventHandlerContainer = new EventHandlerContainer(services);

            action.Invoke(eventHandlerContainer);

            return services;
        }

        public static async Task StartConsumer(this IServiceProvider serviceProvider,Func<IEventHandlerManager,Task> func)
        {
            var scope = serviceProvider.CreateAsyncScope(); 

            var eventhandlerManager = scope.ServiceProvider.GetService<IEventHandlerManager>(); 

            await func(eventhandlerManager);
        }
    }
}
