using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Merchello.Core.ObjectResolution;
using Umbraco.Core;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Represents a EventTriggerRegistry
    /// </summary>
    internal sealed class TriggerResolver : MerchelloManyObjectsResolverBase<TriggerResolver, ITrigger>, ITriggerResolver
    {
        private static readonly ConcurrentDictionary<Guid, ITrigger> TriggerCache = new ConcurrentDictionary<Guid, ITrigger>();

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
        /// <param name="trigger">The <see cref="IBroadcaster"/> to cache</param>
        private static void CacheMapper(Guid key, ITrigger trigger)
        {
            TriggerCache.AddOrUpdate(key, trigger, (x, y) => trigger);
        }


        /// <summary>
        /// Gets a <see cref="IBroadcaster"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        public ITrigger TryGetTrigger(Guid key)
        {
            ITrigger value;
            return TriggerCache.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets a collection of <see cref="IBroadcaster"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        public IEnumerable<ITrigger> GetTriggersByArea(string area)
        {
            return
                GetAllEventTriggers()
                    .Where(x => x.GetType()
                        .GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault(y => y.Area.Equals(area)) != null);            
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IBroadcaster"/>s
        /// </summary>
        public IEnumerable<ITrigger> GetAllEventTriggers()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<ITrigger> Values
        {
            get
            {
                var ctrArgs = new object[] { };
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