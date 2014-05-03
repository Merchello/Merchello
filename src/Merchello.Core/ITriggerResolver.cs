using System;

namespace Merchello.Core
{
    internal interface ITriggerResolver
    {
        void Handle(Type service, EventArgs args);
    }
}