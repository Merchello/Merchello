using System.Collections.Generic;
using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    /// <summary>
    /// EventArgs for Customer conversion
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConvertEventArgs<T> : CancellableObjectEventArgs<IEnumerable<T>>
    {
        /// <summary>
        /// Constructor accepting a single entity instance
        /// </summary>
        public ConvertEventArgs(T eventObject)
            : base(new List<T> { eventObject })
        { }
        
        /// <summary>
        /// Constructor accepting a single entity instance
        /// </summary>
        /// <param name="eventObject"></param>
        /// <param name="canCancel"></param>
        public ConvertEventArgs(T eventObject, bool canCancel)
            : base(new List<T> { eventObject }, canCancel)
        { }

        /// <summary>
        /// Constructor accepting multiple entities that are used in the converting operation
        /// </summary>
        /// <param name="eventObject"></param>
        public ConvertEventArgs(IEnumerable<T> eventObject)
            : base(eventObject)
        { }
  
        /// <summary>
        /// Constructor accepting multiple entities that are used in the converting operation
        /// </summary>
        /// <param name="eventObject"></param>
        /// <param name="canCancel"></param>
        public ConvertEventArgs(IEnumerable<T> eventObject, bool canCancel)
            : base(eventObject, canCancel)
        { }
  
        public IEnumerable<T> CovertedEntities { get { return EventObject; } }
    }
}
