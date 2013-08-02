
using System.Collections.Generic;

namespace Merchello.Core.Events
{
    public class SaveEventArgs<T> : EventArgsBase<IEnumerable<T>>
    {
        /// <summary>
        /// Constructor for accepting multiple entities that are used in the saving operation
        /// </summary>
        /// <param name="eventObject"></param>
        public SaveEventArgs(IEnumerable<T> eventObject)
            : base(eventObject)
        { }

        /// <summary>
        /// Constructor accepting a single entity reference
        /// </summary
        public SaveEventArgs(T eventObject)
            : base(new List<T> {eventObject})
        { }

        public IEnumerable<T> SavedEntities { get { return EventObject; } }
    }
}
