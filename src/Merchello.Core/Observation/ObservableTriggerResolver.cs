using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Observation;
using Umbraco.Core;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Represents a EventTriggerRegistry
    /// </summary>
    internal sealed class ObservableTriggerResolver : MerchelloManyObjectsResolverBase<ObservableTriggerResolver, IObservableTrigger>, IObservableTriggerResolver
    {
        private static readonly ConcurrentDictionary<Guid, IObservableTrigger> TriggerCache = new ConcurrentDictionary<Guid, IObservableTrigger>();

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
                var att = trigger.GetType().GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault();
                if(att != null)
                CacheMapper(att.Key, trigger);
            }

            IsInitialized = true;
        }

        /// <summary>
        /// Adds a key value pair to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="observableTrigger">The <see cref="IObservableTrigger"/> to cache</param>
        private static void CacheMapper(Guid key, IObservableTrigger observableTrigger)
        {
            TriggerCache.AddOrUpdate(key, observableTrigger, (x, y) => observableTrigger);
        }


        /// <summary>
        /// Gets a <see cref="IObservableTrigger"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IObservableTrigger"/></returns>
        public IObservableTrigger TryGetTrigger(Guid key)
        {
            IObservableTrigger value;
            return TriggerCache.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets a collection of <see cref="IObservableTrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IObservableTrigger"/></returns>
        public IEnumerable<IObservableTrigger> GetTriggersByArea(string area)
        {
            return
                GetAllTriggers()
                    .Where(x => x.GetType()
                        .GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault(y => y.Area.Equals(area)) != null);            
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IObservableTrigger"/>s
        /// </summary>
        public IEnumerable<IObservableTrigger> GetAllTriggers()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<IObservableTrigger> Values
        {
            get
            {
                var ctrArgs = new object[] { };
                var triggers = new List<IObservableTrigger>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IObservableTrigger>(et, ctrArgs);
                    if (attempt.Success) triggers.Add(attempt.Result);
                }

                return triggers;
            }
        }
    }
}