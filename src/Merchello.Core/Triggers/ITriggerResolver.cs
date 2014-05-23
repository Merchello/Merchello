using System;
using System.Collections.Generic;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Defines a TriggerRegistry
    /// </summary>
    internal interface ITriggerResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="IBroadcaster"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        IEnumerable<ITrigger> GetTriggersByArea(string area);

        /// <summary>
        /// Gets the collection of all resovled <see cref="IBroadcaster"/>s
        /// </summary>
        IEnumerable<ITrigger> GetAllEventTriggers();

        /// <summary>
        /// Gets a <see cref="IBroadcaster"/> from the resolver
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        ITrigger TryGetTrigger(Guid key);
    }
}