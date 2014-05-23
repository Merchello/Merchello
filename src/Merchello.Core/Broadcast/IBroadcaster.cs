using System;
using System.Collections.Generic;

namespace Merchello.Core.Broadcast
{
    public interface IBroadcaster<in T> : IBroadcaster
    {
        void Broadcast(Guid triggerKey, T model);

        void Broadcast(Guid triggerKey, T model, IEnumerable<string> contacts);
       
    }

    /// <summary>
    /// Marker interface for 
    /// </summary>
    public interface IBroadcaster
    { }
}