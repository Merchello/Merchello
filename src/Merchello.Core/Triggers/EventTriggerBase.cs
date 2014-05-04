using System;
using Merchello.Core.Triggers.Notification;

namespace Merchello.Core.Triggers
{
    public abstract class EventTriggerBase : IEventTrigger
    {
       
        public abstract void Invoke(Object sender, EventArgs e);
    }
}