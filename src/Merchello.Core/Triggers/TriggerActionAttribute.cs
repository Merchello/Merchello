using System;

namespace Merchello.Core.Triggers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TriggerActionAttribute : Attribute
    {
        public string Area { get; private set; }

        public Type Service { get; private set; }

        public string EventName { get; private set; }

        public TriggerActionAttribute(string area, Type service, string eventName)
        {
            Area = area;
            Service = service;
            EventName = eventName;
        }

    }
}