using System.Collections.Generic;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Defines a TriggerRegistry
    /// </summary>
    internal interface IEventTriggerRegistry
    {
        /// <summary>
        /// Gets a collection of <see cref="IEventTrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IEventTrigger"/></returns>
        IEnumerable<IEventTrigger> GetTriggersByArea(string area);

        /// <summary>
        /// Gets the collection of all resovled <see cref="IEventTrigger"/>s
        /// </summary>
        IEnumerable<IEventTrigger> GetAllEventTriggers();

        /// <summary>
        /// Gets a <see cref="IEventTrigger"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IEventTrigger"/></returns>
        IEventTrigger TryGetTrigger(string key);
    }
}