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
        /// Gets a collection of <see cref="ITrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="ITrigger"/></returns>
        IEnumerable<ITrigger> GetTriggersByArea(string area);

        /// <summary>
        /// Gets the collection of all resovled <see cref="ITrigger"/>s
        /// </summary>
        IEnumerable<ITrigger> GetAllEventTriggers();

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="ITrigger"/></returns>
        ITrigger TryGetTrigger(Guid key);
    }
}