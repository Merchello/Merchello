using System;
using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Triggers
{
    internal sealed class EventTriggerResolver : LazyManyObjectsResolverBase<EventTriggerResolver, IEventTrigger>
    {
        public EventTriggerResolver(Func<IEnumerable<Type>> triggers)
            : base(triggers)
        { }

        /// <summary>
        /// Gets the resolved collection of <see cref="IEventTrigger"/>
        /// </summary>
        public IEnumerable<IEventTrigger> Triggers
        {
            get { return Values; }
        }    
    }

     
}