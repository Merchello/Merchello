using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines a Trigger Resolver
    /// </summary>
    internal interface ITriggerResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="ITrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="ITrigger"/></returns>
        IEnumerable<ITrigger> GetTriggersByArea(Topic area);

        /// <summary>
        /// Gets a collection <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        IEnumerable<ITrigger> GetTriggersByAlias(string alias);

        /// <summary>
        /// Gets the collection of all resovled <see cref="ITrigger"/>s
        /// </summary>
        IEnumerable<T> GetAllTriggers<T>();

        /// <summary>
        /// Gets the collection of all resovled <see cref="ITrigger"/>s
        /// </summary>
        IEnumerable<ITrigger> GetAllTriggers();

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        T TryGetTrigger<T>();

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        ITrigger TryGetTrigger(Type type);
    }
}