using Umbraco.Core.Events;

namespace Merchello.Core.Events
{
    public class NewEventArgs<T> : CancellableObjectEventArgs<T>
    {
        /// <summary>
        /// Constructor accepting entities in a creating operation
        /// </summary>
        /// <param name="eventObject"></param>
        public NewEventArgs(T eventObject)
            :base(eventObject, true)
        { }        
       
        public T Entity { get { return EventObject; } }
    }
}
