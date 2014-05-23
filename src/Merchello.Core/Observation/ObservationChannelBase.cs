using System;
using Merchello.Core.Events;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Broadcasts a message 
    /// </summary>
    public abstract class ObservationChannelBase<T> where T : EventArgs
    {
        public static event EventHandler<ObservationChannelEventArgs<T>> Broadcasting;

        protected virtual void OnBroadcasting(object model)
        {
            if (!WillWork(model)) return;
            if (Broadcasting == null) return;
            Broadcasting(this, new ObservationChannelEventArgs<T>((T)model));
        }

        /// <summary>
        /// Returns true/false if the value passed matches the type of model that can
        /// be passed as an EventArgs of T
        /// </summary>
        /// <param name="model">The object type to compair</param>
        private static bool WillWork(object model)
        {
            return typeof(T) == model.GetType();
        }
    }
}