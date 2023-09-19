using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Channels;

namespace Y.EventBus
{
    public class EventHandlerManager : IEventHandlerManager,IDisposable 
    {
        private ConcurrentDictionary<string, Channel<string>> Channels = new ConcurrentDictionary<string, Channel<string>>();

        private bool IsDiposed = false;

        private readonly IServiceProvider ServiceProvider;

        private readonly CancellationToken _cancellation;

        private readonly IEventHandlerContainer _eventHandlerContainer;

        private readonly ILogger _logger;

        private ConcurrentDictionary<string,EventTrigger> EventTriggers;

        private bool IsInitConsumer = true;

        public EventHandlerManager( IServiceProvider serviceProvider
            , IEventHandlerContainer eventHandlerContainer
            , ILoggerFactory loggerFactory)
        {
            ServiceProvider = serviceProvider;
            _cancellation = CancellationToken.None;
            _eventHandlerContainer = eventHandlerContainer;
            EventTriggers = new ConcurrentDictionary<string, EventTrigger>();
            _logger = loggerFactory.CreateLogger<IEventHandlerManager>();
        }

        public async Task CreateChannles()
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

                _logger.LogInformation($"创建通信管道{item.EtoType}--{attribute.EventName}");
            }
            await Task.CompletedTask;
        }

        private Channel<string> Check(Type type)
        {
            var attribute = type .GetCustomAttributes()
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
            IsInitConsumer = true;
            foreach(var trigger in EventTriggers.Values)
            {
                trigger.Dispose();
            }
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

            while ( await channel.Writer.WaitToWriteAsync(CancellationToken.None)) 
            {
                var data = JsonConvert.SerializeObject(eto);

                await channel.Writer.WriteAsync(data, _cancellation);
            }          
        }
        /// <summary>
        /// 消费者
        /// </summary>
        /// <returns></returns>
        public void Subscribe<TEto>() where TEto : class
        {
            var attribute = typeof(TEto).GetCustomAttributes()
           .OfType<EventDiscriptorAttribute>()
           .FirstOrDefault();

            if (attribute is null)
            {
                ThorwEventAttributeNullException.ThorwException();
            }

            if (EventTriggers.Keys.Any(p => p == attribute.EventName))
            {
                return;
            }

            Func<Task> func = async () =>
            {
                var scope = ServiceProvider.CreateAsyncScope();

                var channel = Check(typeof(TEto));

                var handler = scope.ServiceProvider.GetRequiredService<IEventHandler<TEto>>();

                var reader = channel.Reader;

                try
                {
                    while (await channel.Reader.WaitToReadAsync())
                    {
                        while (reader.TryRead(out string str))
                        {
                            var data = JsonConvert.DeserializeObject<TEto>(str);

                            _logger.LogInformation(str);

                            await handler.HandelrAsync(data);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogInformation($"本地事件总线异常{e.Source}--{e.Message}--{e.Data}");
                    throw;
                }
            };

            var trigger = new EventTrigger();
            trigger.Recived(func);

            EventTriggers.TryAdd(attribute.EventName, trigger);
        }

        public Task Trigger()
        {
            //只允许初始化一次消费者
            if (IsInitConsumer)
            {
                foreach (var eventTrigger in EventTriggers)
                {
                    Task.Factory.StartNew(async () =>
                    {
                        await eventTrigger.Value.Trigger();
                    });
                }
            }
            IsInitConsumer = false;
            return Task.CompletedTask;  
        }
    }
}
