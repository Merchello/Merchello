using System.Collections.Generic;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Defines a TriggerRegistry
    /// </summary>
    internal interface IEventTriggeredActionResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="IEventTriggeredAction"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IEventTriggeredAction"/></returns>
        IEnumerable<IEventTriggeredAction> GetTriggersByArea(string area);

        /// <summary>
        /// Gets the collection of all resovled <see cref="IEventTriggeredAction"/>s
        /// </summary>
        IEnumerable<IEventTriggeredAction> GetAllEventTriggers();

        /// <summary>
        /// Gets a <see cref="IEventTriggeredAction"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IEventTriggeredAction"/></returns>
        IEventTriggeredAction TryGetTrigger(string key);
    }
}