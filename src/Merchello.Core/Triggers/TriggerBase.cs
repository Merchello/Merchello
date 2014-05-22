using System;

namespace Merchello.Core.Triggers
{
    internal abstract class TriggerBase : ITrigger
    {       
        public abstract void Invoke(Object sender, EventArgs e);
    }
}