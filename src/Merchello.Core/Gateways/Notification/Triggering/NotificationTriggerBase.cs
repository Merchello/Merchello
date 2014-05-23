using System;
using System.Collections.Generic;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    public class NotificationTriggerBase<T> : IObservable<T>
    {
        private readonly List<IObserver<T>> _observers = new List<IObserver<T>>(); 

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if(!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber<T>(_observers, observer);
        }

        public virtual void Notify(object model)
        {
            if (!WillWork(model)) return;            
            foreach (var o in _observers)
            {
                try
                {
                    o.OnNext((T)model);
                }
                catch (Exception ex)
                {                    
                    o.OnError(ex);
                }               
            }
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