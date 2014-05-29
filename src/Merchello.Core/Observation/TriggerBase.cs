using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines an <see cref="TriggerBase{T}"/> base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TriggerBase<T> : IObservableTrigger<T>
    {
        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            if (!Observers.Contains(observer)) Observers.Add(observer);
            return GetUnsubscriber(observer);
        }

        /// <summary>
        /// Gets the collection of <see cref="IObserver{T}"/>
        /// </summary>
        protected List<IObserver<T>> Observers = new List<IObserver<T>>();


        protected IDisposable GetUnsubscriber(IObserver<T> observer)
        {
            return new Unsubscriber<T>(Observers, observer);
        }

        /// <summary>
        /// Notifiy all the monitors of the change
        /// </summary>
        /// <param name="monitorModel">The model/value to pass to each monitor</param>
        protected virtual void NotifyMonitors(T monitorModel)
        {
            foreach (var o in Observers)
            {
                try
                {
                    o.OnNext(monitorModel);
                }
                catch (Exception ex)
                {
                    o.OnError(ex);
                }
            }
        }

        /// <summary>
        /// Returns true/false indicating wether or not the model passed "Will Work" for this trigger
        /// </summary>
        /// <param name="model">An object representing the model to be passed to the various Monitors</param>
        internal virtual bool WillWork(object model)
        {
            return model == null || model.GetType().IsAssignableFrom(typeof (T));
        }
    }
}