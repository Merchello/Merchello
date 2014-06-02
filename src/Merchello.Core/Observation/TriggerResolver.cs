using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.ObjectResolution;
using Umbraco.Core;
using Umbraco.Core.Logging;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Represents a TriggerResolver
    /// </summary>
    internal sealed class TriggerResolver : MerchelloManyObjectsResolverBase<TriggerResolver, ITrigger>, ITriggerResolver
    {
        private static readonly ConcurrentDictionary<Type, ITrigger> TriggerCache = new ConcurrentDictionary<Type, ITrigger>();

        internal static bool IsInitialized { get; private set; }

        internal TriggerResolver(IEnumerable<Type> triggers)
            : base(triggers)
        {
            BuildCache();
        }

        private void BuildCache()
        {
            foreach (var trigger in Values)
            {              
                CacheMapper(trigger.GetType(), trigger);
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Adds a key value pair to the dictionary
        /// </summary>        
        /// <param name="type">The trigger of the trigger</param>
        /// <param name="observableTrigger">The <see cref="ITrigger"/> to cache</param>
        private static void CacheMapper(Type type, ITrigger observableTrigger)
        {
            TriggerCache.AddOrUpdate(type, observableTrigger, (x, y) => observableTrigger);
        }

        /// <summary>
        /// Gets a collection of <see cref="ITrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="topic">The "area"</param>
        /// <returns>A <see cref="ITrigger"/></returns>
        public IEnumerable<ITrigger> GetTriggersByArea(Topic topic)
        {
            return  TriggerCache.Values
                .Where(x => x.GetType()
                        .GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault(y => y.Topic == topic) != null);   
        }

        /// <summary>
        /// Gets a collection <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        public IEnumerable<ITrigger> GetTriggersByAlias(string alias, Topic topic = Topic.Notifications)
        {

            return TriggerCache.Values.Where(x => (x.GetType().GetCustomAttribute<TriggerForAttribute>(false) != null &&
                                      String.Equals(x.GetType().GetCustomAttribute<TriggerForAttribute>(false).Alias, alias, StringComparison.InvariantCultureIgnoreCase) &&
                                      x.GetType().GetCustomAttribute<TriggerForAttribute>(false).Topic == topic
                                      ))
                         .Select(trigger => trigger);
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="ITrigger"/>s of a particular type
        /// </summary>
        public IEnumerable<T> GetAllTriggers<T>()
        {
            return TriggerCache.Values.Where(x => x.GetType().IsAssignableFrom(typeof(T))).Select(x => (T)x);
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="ITrigger"/>s
        /// </summary>
        public IEnumerable<ITrigger> GetAllTriggers()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        public T GetTrigger<T>()
        {
            return (T)GetTrigger(typeof (T));
        }

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        public ITrigger GetTrigger(Type type)
        {
            if (!TriggerCache.ContainsKey(type))
            {
                LogHelper.Debug<TriggerResolver>(string.Format("A trigger with type {0} could not be found.", type.Name));
            }

            return TriggerCache[type];
        }
        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<ITrigger> Values
        {
            get
            {
                var ctrArgs = new object[] {};
                var triggers = new List<ITrigger>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<ITrigger>(et, ctrArgs);
                    if (attempt.Success) triggers.Add(attempt.Result);
                }

                return triggers;
            }
        }
    }
}