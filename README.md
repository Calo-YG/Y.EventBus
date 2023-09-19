## Y.EventBus

基于.NET 平台 C#	语言 提供的Channel打造的异步事件总线库

## [Channel使用](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/channels)

### [源码链接](https://github.com/Calo-YG/Y.EventBus)

### 使用

EventDiscriptorAttribute 特性

```csharp
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = false)]
    public class EventDiscriptorAttribute:Attribute
    {
       /// <summary>
       /// 事件2名称
       /// </summary>
       public string EventName { get; private set; }
       /// <summary>
       /// channel 容量设置
       /// </summary>
       public int Capacity { get; private set; }  
       /// <summary>
       /// 是否维持一个生产者多个消费者模型
       /// </summary>
       public bool SigleReader { get; private set; }

       public EventDiscriptorAttribute(string eventName, int capacity = 1000, bool sigleReader = true)
        {
            EventName = eventName;
            Capacity = capacity;
            SigleReader = sigleReader;
        }   
    }
```

Eto 实现特性

```csharp
    [EventDiscriptor("test",1000,false)]
    public class TestEto
    {
        public string Name { get; set; }    

        public string Description { get; set; } 
    }
```

添加通信管道

```csharp
context.Services.AddChannles(p =>
{
    p.TryAddChannle<TestEto>();
});
```

注入EventBus

```csharp
context.Services.AddEventBus();
```

创建订阅Eto

```csharp
var scope = context.ServiceProvider.CreateScope();

var eventhandlerManager = scope.ServiceProvider.GetRequiredService<IEventHandlerManager>();

await authorizeManager.AddAuthorizeRegiester();

await eventhandlerManager.CreateChannles();

eventhandlerManager.Subscribe<TestEto>();
```

EventHandler定义

```csharp
    public class TestEventHandler : IEventHandler<TestEto>,ITransientInjection
    {
        private ILogger _logger;
        public TestEventHandler(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<TestEventHandler>();
        }   
        public Task HandelrAsync(TestEto eto)
        {
            _logger.LogInformation($"{typeof(TestEto).Name}--{eto.Name}--{eto.Description}");
            return Task.CompletedTask;
        }
    }
    //注意EventHandler的生命周期请使用AddTrasint() 或者AddScoped()
```

```csharp
//构造函数注入即可使用
TestEto eto = null;

for(var i = 0; i < 100; i++)
{
        eto = new TestEto()
        {
            Name ="LocalEventBus" + i.ToString(),
            Description ="wyg"+i.ToString(),
        };
        await _localEventBus.PublichAsync(eto,CancellationToken.None);
}
```

![image](https://github.com/Calo-YG/Y.EventBus/assets/74019004/bfabb05b-518d-4699-bfd1-18b711659c88)


