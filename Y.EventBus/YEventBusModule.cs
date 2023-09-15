using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Y.Module;
using Y.Module.Extensions;
using Y.Module.Modules;

namespace Y.EventBus
{
    public class YEventBusModule:YModule
    {
        public override void ConfigerService(ConfigerServiceContext context)
        {
            context.Services.AddAssembly(Assembly.GetExecutingAssembly());
        }

        public override async Task LaterInitApplicationAsync(InitApplicationContext context)
        {
            var scope = context.ServiceProvider.CreateAsyncScope();

            var eventhandlerManager = scope.ServiceProvider.GetRequiredService<IEventHandlerManager>();

            await eventhandlerManager.Subcrice();
        }
    }
}
