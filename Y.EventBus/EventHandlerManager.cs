using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;
using Y.Module.DependencyInjection;

namespace Y.EventBus
{
    public class EventHandlerManager : IEventHandlerManager,ISingletonInjection
    {
        private ConcurrentDictionary<string, Channel<string>> Channels = new ConcurrentDictionary<string, Channel<string>>();

        private bool IsDiposed = false;

        private readonly IServiceProvider ServiceProvider;

        private readonly CancellationToken _cancellation;

        private readonly IEventHandlerContainer _eventHandlerContainer;

        private readonly ILogger _logger;

        public EventHandlerManager( IServiceProvider serviceProvider
            , IEventHandlerContainer eventHandlerContainer
            , ILoggerFactory loggerFactory)
        {
            ServiceProvider = serviceProvider;
            _cancellation = CancellationToken.None;
            _eventHandlerContainer = eventHandlerContainer;
            _logger = loggerFactory.CreateLogger<IEventHandlerManager>();
        }

        public async Task Subcrice()
        {
            var eventDiscriptions = _eventHandlerContainer.Events;

            foreach(var item in eventDiscriptions)
            {
                var attribute = item.EtoType.GetCustomAttributes()
                                            .OfType<EventDiscriptorAttribute>()
                                            .FirstOrDefault();

                if (attribute is null)
                {
                    ThorwEventAttributeNullException.ThorwException();
                }

                var channel = Channels.GetValueOrDefault(attribute.EventName);

                if (channel is not null)
                {
                    return;
                }

                channel = Channel.CreateBounded<string>(
                        new BoundedChannelOptions(attribute.Capacity)
                              {
                                SingleWriter = true,
                                SingleReader = false,
                                AllowSynchronousContinuations = false,
                                FullMode = BoundedChannelFullMode.Wait
                        });

                Channels.TryAdd(attribute.EventName, channel);
            }
            await Task.CompletedTask;
        }

        private Channel<string> Check(Type type)
        {
            var attribute = typeof(Type).GetCustomAttributes()
                                   .OfType<EventDiscriptorAttribute>()
                                   .FirstOrDefault();

            if (attribute is null)
            {
                ThorwEventAttributeNullException.ThorwException();
            }

            var channel = Channels.GetValueOrDefault(attribute.EventName);

            if(channel is null)
            {
                ThrowChannelNullException.ThrowException(attribute.EventName);
            }

            return channel;
        }

        public void Dispose()
        {
            IsDiposed = true;
            _cancellation.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// 生产者
        /// </summary>
        /// <typeparam name="TEto"></typeparam>
        /// <param name="eto"></param>
        /// <returns></returns>
        public async Task WriteAsync<TEto>(TEto eto) where TEto : class
        {
            var channel = Check(typeof(TEto));

            var data = JsonConvert.SerializeObject(eto);

            await channel.Writer.WriteAsync(data, _cancellation);
        }
        /// <summary>
        /// 消费者
        /// </summary>
        /// <returns></returns>
        public async Task Consumption()
        {
            var baseType = typeof(IEventHandler<>);

            var scope = ServiceProvider.CreateAsyncScope();

            foreach (var item in _eventHandlerContainer.Events)
            {
                _ = Task.Factory.StartNew(async() =>
                {
                    var channel = Check(item.EtoType);

                    var handlertype = baseType.MakeGenericType(item.EtoType);

                    var handler = scope.ServiceProvider.GetRequiredService(handlertype);

                    if(handler is not IEventHandler @eventhandler)
                    {
                        return
                    }

                    var reader = channel.Reader;

                    try
                    {
                        while (await channel.Reader.WaitToReadAsync())
                        {
                            while (reader.TryRead(out string data))
                            {
                                await eventhandler.HandelrAsync(data);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation($"本地事件总线异常{e.Source}--{e.Message}--{e.Data}");
                        throw;
                    }
                });
            }
        }

    }
}
