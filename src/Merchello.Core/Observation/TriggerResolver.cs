using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.ObjectResolution;
using Umbraco.Core;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Represents a EventTriggerRegistry
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
        /// <param name="type">The type of the trigger</param>
        /// <param name="observableTrigger">The <see cref="ITrigger"/> to cache</param>
        private static void CacheMapper(Type type, ITrigger observableTrigger)
        {
            TriggerCache.AddOrUpdate(type, observableTrigger, (x, y) => observableTrigger);
        }

        /// <summary>
        /// Gets a collection of <see cref="ITrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="ITrigger"/></returns>
        public IEnumerable<T> GetTriggersByArea<T>(ObservableTopic area)
        {
            return  GetAllTriggers<T>()
                .Where(x => x.GetType()
                        .GetCustomAttributes<ObservableTriggerForAttribute>(false).FirstOrDefault(y => y.Area == area) != null);   
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="ITrigger"/>s
        /// </summary>
        public IEnumerable<T> GetAllTriggers<T>()
        {
            return TriggerCache.Values.Where(x => x.GetType().IsAssignableFrom(typeof(T))).Select(x => (T)x);
        }

        /// <summary>
        /// Gets a <see cref="ITrigger"/> from the resolver
        /// </summary>
        /// <returns>A <see cref="ITrigger"/></returns>
        public T TryGetTrigger<T>(Type type)
        {
            return (T)TriggerCache[type];
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