using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Merchello.Core.ObjectResolution;
using Umbraco.Core;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Triggers
{
    /// <summary>
    /// Represents a EventTriggerRegistry
    /// </summary>
    internal sealed class EventTriggerRegistry : LazyManyObjectsResolverBase<EventTriggerRegistry, IEventTriggeredAction>, IEventTriggerRegistry
    {
        private static readonly ConcurrentDictionary<string, IEventTriggeredAction> TriggerCache = new ConcurrentDictionary<string, IEventTriggeredAction>();

        internal static bool IsInitialized { get; private set; }

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

            IsInitialized = true;
        }

        /// <summary>
        /// Adds a key value pair to the dictionary
        /// </summary>
        /// <param name="key"></param>
        /// <param name="triggeredAction">The <see cref="IEventTriggeredAction"/> to cache</param>
        private static void CacheMapper(string key, IEventTriggeredAction triggeredAction)
        {
            TriggerCache.AddOrUpdate(key, triggeredAction, (x, y) => triggeredAction);
        }


        /// <summary>
        /// Gets a <see cref="IEventTriggeredAction"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IEventTriggeredAction"/></returns>
        public IEventTriggeredAction TryGetTrigger(string key)
        {
            IEventTriggeredAction value;
            return TriggerCache.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets a collection of <see cref="IEventTriggeredAction"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IEventTriggeredAction"/></returns>
        public IEnumerable<IEventTriggeredAction> GetTriggersByArea(string area)
        {
            return
                GetAllEventTriggers()
                    .Where(x => x.GetType().GetCustomAttributes<EventTriggeredActionForAttribute>(false).FirstOrDefault().Area.Equals(area));            
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IEventTriggeredAction"/>s
        /// </summary>
        public IEnumerable<IEventTriggeredAction> GetAllEventTriggers()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets the resolved collection of <see cref="IEventTriggeredAction"/>
        /// </summary>
        /// <remarks>
        /// Should be able to use the "Values" in the base class but it can't be tested
        /// due to the Umbraco's <see cref="ObjectResolution.Resolution"/> 
        /// </remarks>
        internal IEnumerable<IEventTriggeredAction> EventTriggers
        {
            get
            {
                var ctrArgs = new object[] {};
                var triggers = new List<IEventTriggeredAction>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IEventTriggeredAction>(et.AssemblyQualifiedName, ctrArgs);
                    if (attempt.Success) triggers.Add(attempt.Result);
                }

                return triggers;
            }
        }
    }
}