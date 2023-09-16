## Y.EventBus

����.NET ƽ̨ C#	���� �ṩ��Channle������첽�¼����߿�

## [Channleʹ��](https://learn.microsoft.com/zh-cn/dotnet/core/extensions/channels)

### [Դ������](https://github.com/Calo-YG/Y.EventBus)

### ʹ��

EventDiscriptorAttribute ����

```csharp
    [AttributeUsage(AttributeTargets.Class,AllowMultiple = false,Inherited = false)]
    public class EventDiscriptorAttribute:Attribute
    {
       /// <summary>
       /// �¼�2����
       /// </summary>
       public string EventName { get; private set; }
       /// <summary>
       /// channel ��������
       /// </summary>
       public int Capacity { get; private set; }  
       /// <summary>
       /// �Ƿ�ά��һ�������߶��������ģ��
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

Eto ʵ������

```csharp
    [EventDiscriptor("test",1000,false)]
    public class TestEto
    {
        public string Name { get; set; }    

        public string Description { get; set; } 
    }
```

���ͨ�Źܵ�

```csharp
context.Services.AddChannles(p =>
{
    p.TryAddChannle<TestEto>();
});
```

ע��EventBus

```csharp
context.Services.AddEventBus();
```

��������Eto

```csharp
var scope = context.ServiceProvider.CreateScope();

var eventhandlerManager = scope.ServiceProvider.GetRequiredService<IEventHandlerManager>();

await authorizeManager.AddAuthorizeRegiester();

await eventhandlerManager.CreateChannles();

eventhandlerManager.Subscribe<TestEto>();
```

EventHandler����

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
    //ע��EventHandler������������ʹ��AddTrasint() ����AddScoped()
```
