using System;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Defines a 
    /// </summary>
    internal interface IEventTrigger
    {
        void Invoke(Object sender, EventArgs e);
    }
}