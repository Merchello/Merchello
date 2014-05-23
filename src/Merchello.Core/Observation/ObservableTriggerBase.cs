using System;
using System.Collections.Generic;

namespace Merchello.Core.Observation
{
    public abstract class ObservableTriggerBase<T> : IObservableTrigger<T>
    {
        protected List<IObserver<T>> Observers = new List<IObserver<T>>(); 

        public abstract IDisposable Subscribe(IObserver<T> observer);

        protected IDisposable GetUnsubscriber(IObserver<T> observer)
        {
            return new Unsubscriber<T>(Observers, observer);
        }
    }
}