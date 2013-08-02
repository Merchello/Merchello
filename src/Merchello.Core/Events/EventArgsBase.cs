
namespace Merchello.Core.Events
{
    public abstract class EventArgsBase<T>
    {
        protected EventArgsBase(T entity)
        {
            EventObject = entity;
        }

        /// <summary>
        /// Returns the object relating to the event 
        /// </summary>
        protected T EventObject { get; private set; }
    }
}
