using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines a TriggerRegistry
    /// </summary>
    internal interface IObservableTriggerResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="IObservableTrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IObservableTrigger"/></returns>
        IEnumerable<IObservableTrigger> GetTriggersByArea(string area);

        /// <summary>
        /// Gets the collection of all resovled <see cref="IObservableTrigger"/>s
        /// </summary>
        IEnumerable<IObservableTrigger> GetAllTriggers();

        /// <summary>
        /// Gets a <see cref="IObservableTrigger"/> from the resolver
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IObservableTrigger"/></returns>
        IObservableTrigger TryGetTrigger(Guid key);
    }
}