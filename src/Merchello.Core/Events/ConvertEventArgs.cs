using System.Collections.Generic;

namespace Merchello.Core.Events
{
    public class ConvertEventArgs<T> : EventArgsBase<IEnumerable<T>>
    {
        /// <summary>
        /// Constructor accepting a single entity instance
        /// </summary>
        /// <param name="eventObject"></param>
        public ConvertEventArgs(T eventObject)
            : base(new List<T>{ eventObject })
        { }

        /// <summary>
        /// Constructor accepting multiple entities that are used in the converting operation
        /// </summary>
        /// <param name="eventObject"></param>
        public ConvertEventArgs(IEnumerable<T> eventObject)
            : base(eventObject)
        { }
      
        public IEnumerable<T> CovertedEntities { get { return EventObject; } }
    }
}
