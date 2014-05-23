using System;

namespace Merchello.Core.Triggers
{
    internal interface ITrigger
    {
        void Invoke(Object sender, EventArgs e);
    }
}