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
    internal sealed class ObservableTriggerResolver : MerchelloManyObjectsResolverBase<ObservableTriggerResolver, ITrigger>, IObservableTriggerResolver
    {
        private static readonly ConcurrentDictionary<Type, ITrigger> TriggerCache = new ConcurrentDictionary<Type, ITrigger>();

        internal static bool IsInitialized { get; private set; }

        internal ObservableTriggerResolver(IEnumerable<Type> triggers)
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


        public IEnumerable<T> GetTriggersByArea<T>(ObservableTopic area) where T : ITrigger
        {
            return  GetAllTriggers<T>()
                .Where(x => x.GetType()
                        .GetCustomAttributes<ObservableTriggerForAttribute>(false).FirstOrDefault(y => y.Area == area) != null);   
        }

        public IEnumerable<T> GetAllTriggers<T>() where T : ITrigger
        {
            return TriggerCache.Values.Where(x => x.GetType().IsAssignableFrom(typeof(T))).Select(x => (T)x);
        }

        public T TryGetTrigger<T>(Type type) where T : ITrigger
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