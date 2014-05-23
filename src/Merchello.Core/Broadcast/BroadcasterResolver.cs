using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Triggers;
using Umbraco.Core;

namespace Merchello.Core.Broadcast
{
    /// <summary>
    /// Represents a EventTriggerRegistry
    /// </summary>
    internal sealed class BroadcasterResolver : MerchelloManyObjectsResolverBase<BroadcasterResolver, IBroadcaster>, IBroadcasterResolver
    {
        private static readonly ConcurrentDictionary<Guid, IBroadcaster> TriggerCache = new ConcurrentDictionary<Guid, IBroadcaster>();

        internal static bool IsInitialized { get; private set; }

        internal BroadcasterResolver(IEnumerable<Type> triggers)
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
        /// <param name="broadcaster">The <see cref="IBroadcaster"/> to cache</param>
        private static void CacheMapper(Guid key, IBroadcaster broadcaster)
        {
            TriggerCache.AddOrUpdate(key, broadcaster, (x, y) => broadcaster);
        }


        /// <summary>
        /// Gets a <see cref="IBroadcaster"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        public IBroadcaster TryGetBroadcaster(Guid key)
        {
            IBroadcaster value;
            return TriggerCache.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets a collection of <see cref="IBroadcaster"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IBroadcaster"/></returns>
        public IEnumerable<IBroadcaster> GetBroadcastersByArea(string area)
        {
            return
                GetAllBroadcasters()
                    .Where(x => x.GetType()
                        .GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault(y => y.Area.Equals(area)) != null);            
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IBroadcaster"/>s
        /// </summary>
        public IEnumerable<IBroadcaster> GetAllBroadcasters()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<IBroadcaster> Values
        {
            get
            {
                var ctrArgs = new object[] { };
                var triggers = new List<IBroadcaster>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IBroadcaster>(et, ctrArgs);
                    if (attempt.Success) triggers.Add(attempt.Result);
                }

                return triggers;
            }
        }
    }
}