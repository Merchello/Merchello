using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines a Trigger Resolver
    /// </summary>
    public interface ITriggerResolver
    {
        /// <summary>
        /// Gets a collection of <see cref="ITrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="topic">The "area"</param>
        /// <returns>A <see cref="ITrigger"/></returns>
        IEnumerable<ITrigger> GetTriggersByArea(Topic topic);

        /// <summary>
        /// Gets a collection <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        /// <remarks>
        /// 
        /// By design there should only ever be one of these per alias, but someone might think
        /// of something we have'nt
        /// 
        /// </remarks>
        IEnumerable<ITrigger> GetTriggersByAlias(string alias, Topic topic = Topic.Notifications);

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
        T GetTrigger<T>();

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        ITrigger GetTrigger(Type type);
    }
}