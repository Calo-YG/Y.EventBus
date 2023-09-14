﻿namespace Y.EventBus
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EventDiscriptorAttribute:Attribute
    {
       /// <summary>
       /// 事件2名称
       /// </summary>
       public string EventName { get; private set; }
       /// <summary>
       /// channel 容量设置
       /// </summary>
       public int Capacity { get; private set; } = 10000;  
       /// <summary>
       /// 是否维持一个生产者多个消费者模型
       /// </summary>
       public bool SigleReader { get; private set; } = true;

       public EventDiscriptorAttribute(string eventName, int capacity, bool sigleReader)
        {
            EventName = eventName;
            Capacity = capacity;
            SigleReader = sigleReader;
        }   
    }
}
