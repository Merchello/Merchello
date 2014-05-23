using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.ObjectResolution;
using Merchello.Core.Triggers;
using Umbraco.Core;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Represents a BroadcasterResolver
    /// </summary>
    internal sealed class ObservationChannelResolver : MerchelloManyObjectsResolverBase<ObservationChannelResolver, IObservationChannel>, IObservationChannelResolver
    {
        private static readonly ConcurrentDictionary<Guid, IObservationChannel> TriggerCache = new ConcurrentDictionary<Guid, IObservationChannel>();

        internal static bool IsInitialized { get; private set; }

        internal ObservationChannelResolver(IEnumerable<Type> triggers)
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
        /// <param name="observationChannel">The <see cref="IObservationChannel"/> to cache</param>
        private static void CacheMapper(Guid key, IObservationChannel observationChannel)
        {
            TriggerCache.AddOrUpdate(key, observationChannel, (x, y) => observationChannel);
        }


        /// <summary>
        /// Gets a <see cref="IObservationChannel"/> from the registry
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A <see cref="IObservationChannel"/></returns>
        public IObservationChannel TryGetChannel(Guid key)
        {
            IObservationChannel value;
            return TriggerCache.TryGetValue(key, out value) ? value : null;
        }

        /// <summary>
        /// Gets a collection of <see cref="IObservationChannel"/> by the area defined in the attribute
        /// </summary>
        /// <param name="area">The "area"</param>
        /// <returns>A <see cref="IObservationChannel"/></returns>
        public IEnumerable<IObservationChannel> GetChannelByArea(string area)
        {
            return
                GetAllChannels()
                    .Where(x => x.GetType()
                        .GetCustomAttributes<TriggerForAttribute>(false).FirstOrDefault(y => y.Area.Equals(area)) != null);            
        }

        /// <summary>
        /// Gets the collection of all resovled <see cref="IObservationChannel"/>s
        /// </summary>
        public IEnumerable<IObservationChannel> GetAllChannels()
        {
            return TriggerCache.Values;
        }

        /// <summary>
        /// Gets the instantiated values of the resolved types
        /// </summary>
        protected override IEnumerable<IObservationChannel> Values
        {
            get
            {
                var ctrArgs = new object[] { };
                var triggers = new List<IObservationChannel>();

                foreach (var et in InstanceTypes)
                {
                    var attempt = ActivatorHelper.CreateInstance<IObservationChannel>(et, ctrArgs);
                    if (attempt.Success) triggers.Add(attempt.Result);
                }

                return triggers;
            }
        }
    }
}