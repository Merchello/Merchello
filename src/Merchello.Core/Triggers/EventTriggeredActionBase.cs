using System;

namespace Merchello.Core.Triggers
{
    public abstract class EventTriggeredActionBase : IEventTriggeredAction
    {
       
        public abstract void Invoke(Object sender, EventArgs e);

    }
}