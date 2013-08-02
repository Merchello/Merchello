namespace Merchello.Core.Events
{
    public class NewEventArgs<T> : EventArgsBase<T>
    {
        /// <summary>
        /// Constructor accepting entities in a creating operation
        /// </summary>
        /// <param name="eventObject"></param>
        public NewEventArgs(T eventObject)
            :base(eventObject)
        { }        
       
        public T Entity { get { return EventObject; } }
    }
}
