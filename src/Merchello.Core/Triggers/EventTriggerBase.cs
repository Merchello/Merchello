using System;

namespace Merchello.Core.Triggers
{
    public abstract class EventTriggerBase : IEventTrigger
    {
       
        public abstract void Invoke(Object sender, EventArgs e);

    }
}