using System;

namespace Merchello.Core
{
    internal interface IEventTriggerBase
    {
        void Invoke(Type service, EventArgs args);
    }
}