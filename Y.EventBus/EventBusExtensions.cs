using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using Y.Module.Extensions;

namespace Y.EventBus
{
    public  static class EventBusExtensions
    {
        //添加事件总线并且添加订阅
        public static IServiceCollection AddEventBusAndSubcrise(this IServiceCollection services,Action<EventHandlerContainer> action)
        {
            services.ChcekNull();

            services.AddSingleton<IEventHandlerManager, EventHandlerManager>();

            services.AddSingleton<ILocalEventBus, LocalEventBus>();

            EventHandlerContainer eventHandlerContainer = new EventHandlerContainer(services);

            action.Invoke(eventHandlerContainer);

            return services;
        }

        //开始消费
        public static async Task StartConsumer(this IServiceProvider serviceProvider,Func<IEventHandlerManager,Task> func)
        {
            var scope = serviceProvider.CreateAsyncScope(); 

            var eventhandlerManager = scope.ServiceProvider.GetService<IEventHandlerManager>();

            await eventhandlerManager.Subcrice();

            await func(eventhandlerManager);
        }

        //添加本地事件总线
        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            services.ChcekNull();

            services.AddSingleton<IEventHandlerManager, EventHandlerManager>();

            services.AddSingleton<ILocalEventBus, LocalEventBus>();

            return services;
        }

        //添加订阅
        public static IServiceCollection AddSubcrise(this IServiceCollection services, Action<EventHandlerContainer> action)
        {
            EventHandlerContainer eventHandlerContainer = new EventHandlerContainer(services);

            action.Invoke(eventHandlerContainer);

            return services;
        }
    }
}
