using System;
using System.Collections.Generic;
using Merchello.Core.Triggers;

namespace Merchello.Core.Broadcast
{
    /// <summary>
    /// Defines a BroadcasterResolver
    /// </summary>
    internal interface IBroadcasterResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="IBroadcaster"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        IEnumerable<IBroadcaster> GetBroadcastersByArea(string area);

        /// <summary>
        /// Gets the collection of all resovled <see cref="IBroadcaster"/>s
        /// </summary>
        IEnumerable<IBroadcaster> GetAllBroadcasters();

        /// <summary>
        /// Gets a <see cref="IBroadcaster"/> from the resolver
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        IBroadcaster TryGetBroadcaster(Guid key);
    }
}