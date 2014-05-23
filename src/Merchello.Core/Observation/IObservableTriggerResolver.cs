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
        /// Gets a collection of <see cref="ITrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="ITrigger"/></returns>
        IEnumerable<T> GetTriggersByArea<T>(ObservableTopic area) where T : ITrigger;

        /// <summary>
        /// Gets the collection of all resovled <see cref="ITrigger"/>s
        /// </summary>
        IEnumerable<T> GetAllTriggers<T>() where T : ITrigger;

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        T TryGetTrigger<T>(Type type) where T : ITrigger;
    }
}