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
    internal sealed class EventTriggeredActionResolver : MerchelloManyObjectsResolverBase<EventTriggeredActionResolver, IEventTriggeredAction>, IEventTriggeredActionResolver
    {
        private static readonly ConcurrentDictionary<string, IEventTriggeredAction> TriggerCache = new ConcurrentDictionary<string, IEventTriggeredAction>();

        internal static bool IsInitialized { get; private set; }

        internal EventTriggeredActionResolver(IEnumerable<Type> triggers)
            : base(triggers)
        {
            BuildCache();
        }

        private void BuildCache()
        {
            foreach (var trigger in Values)
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
                    .Where(x => x.GetType()
                        .GetCustomAttributes<EventTriggeredActionForAttribute>(false).FirstOrDefault(y => y.Area.Equals(area)) != null);            
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IEventTriggeredAction"/>s
        /// </summary>
        public IEnumerable<IEventTriggeredAction> GetAllEventTriggers()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<IEventTriggeredAction> Values
        {
            get
            {
                var ctrArgs = new object[] { };
                var triggers = new List<IEventTriggeredAction>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IEventTriggeredAction>(et, ctrArgs);
                    if (attempt.Success) triggers.Add(attempt.Result);
                }

                return triggers;
            }
        }
    }
}