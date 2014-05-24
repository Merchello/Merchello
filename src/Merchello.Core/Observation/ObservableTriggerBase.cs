using System;
using System.Collections.Generic;
using Merchello.Core.Gateways.Notification;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines an <see cref="ObservableTriggerBase{T}"/> base class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ObservableTriggerBase<T> : IObservableTrigger<T>
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
    }
}