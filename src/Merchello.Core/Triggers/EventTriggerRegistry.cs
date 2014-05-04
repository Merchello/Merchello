using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Umbraco.Core;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Represents a EventTriggerRegistry
    /// </summary>
    internal sealed class EventTriggerRegistry : LazyManyObjectsResolverBase<EventTriggerRegistry, IEventTrigger>, IEventTriggerRegistry
    {
        
        private static readonly ConcurrentDictionary<string, IEventTrigger> TriggerCache = new ConcurrentDictionary<string, IEventTrigger>();

        internal EventTriggerRegistry(Func<IEnumerable<Type>> triggers)
            : base(triggers)
        {
            BuildCache();
        }

        private void BuildCache()
        {
            foreach (var trigger in EventTriggers)
            {
                CacheMapper(trigger.GetType().Name, trigger);
            }
        }

        /// <summary>
        /// Adds a key value pair to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="trigger">The <see cref="IEventTrigger"/> to cache</param>
        private static void CacheMapper(string key, IEventTrigger trigger)
        {
            TriggerCache.AddOrUpdate(key, trigger, (x, y) => trigger);
        }


        /// <summary>
        /// Gets a <see cref="IEventTrigger"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IEventTrigger"/></returns>
        public IEventTrigger TryGetTrigger(string key)
        {
            IEventTrigger value;
            return TriggerCache.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets a collection of <see cref="IEventTrigger"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IEventTrigger"/></returns>
        public IEnumerable<IEventTrigger> GetTriggersByArea(string area)
        {
            return
                GetAllEventTriggers()
                    .Where(x => x.GetType().GetCustomAttributes<TriggerActionAttribute>(false).FirstOrDefault().Area.Equals(area));            
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IEventTrigger"/>s
        /// </summary>
        public IEnumerable<IEventTrigger> GetAllEventTriggers()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets the resolved collection of <see cref="IEventTrigger"/>
        /// </summary>
        /// <remarks>
        /// Should be able to use the "Values" in the base class but it can't be tested
        /// due to the Umbraco's <see cref="Resolution"/> 
        /// </remarks>
        internal IEnumerable<IEventTrigger> EventTriggers
        {
            get
            {
                var ctrArgs = new object[] { };
                var triggers = new List<IEventTrigger>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IEventTrigger>(et.AssemblyQualifiedName, ctrArgs);
                    if (attempt.Success) triggers.Add(attempt.Result);
                }

                return triggers;
            }
        }
    }
}