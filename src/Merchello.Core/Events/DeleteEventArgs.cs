using System.Collections.Generic;

namespace Merchello.Core.Events
{
    public class DeleteEventArgs<T> : EventArgsBase<IEnumerable<T>>
    {
        /// <summary>
        /// Constructor accepting a single entity instance
        /// </summary>
        /// <param name="eventObject"></param>
        public DeleteEventArgs(T eventObject)
            : base(new List<T> { eventObject })
        { }

        /// <summary>
        /// Constructor accepting multiple entities that are used in the delete operation
        /// </summary>
        /// <param name="eventObject"></param>
        public DeleteEventArgs(IEnumerable<T> eventObject)
            : base(eventObject)
        { }
      
        public IEnumerable<T> DeletedEntities { get { return EventObject; } }
    }
}
