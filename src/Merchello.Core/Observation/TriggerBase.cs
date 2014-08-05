namespace Merchello.Core.Observation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Defines an <see cref="TriggerBase{T}"/> base class
    /// </summary>
    /// <typeparam name="T">The type of the model used by the trigger</typeparam>
    public abstract class TriggerBase<T> : IObservableTrigger<T>
    {
        /// <summary>
        /// Gets the collection of <see cref="IObserver{T}"/>
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "Reviewed. Suppression is OK here."), 
        SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")] 
        protected List<IObserver<T>> Observers = new List<IObserver<T>>();


        /// <summary>
        /// Gets a value indicating whether or not this trigger is being monitored
        /// </summary>
        public bool HasMonitors
        {
            get { return Observers.Any(); }
        }

        /// <summary>
        /// Gets the number of monitor observing this trigger
        /// </summary>
        public int MonitorCount
        {
            get { return Observers.Count; }
        }

        /// <summary>
        /// Subscribes an <see cref="IObserver{T}"/>
        /// </summary>
        /// <param name="observer">The monitor</param>
        /// <returns>The disposable</returns>
        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            if (!Observers.Contains(observer)) Observers.Add(observer);
            return GetUnsubscriber(observer);
        }

        /// <summary>
        /// Returns true/false indicating whether or not the model passed "Will Work" for this trigger
        /// </summary>
        /// <param name="model">
        /// An object representing the model to be passed to the various Monitors
        /// </param>
        /// <typeparam name="TModel">The type of the input model</typeparam>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal virtual bool WillWork<TModel>(object model)
        {
            var type = typeof(TModel);

            return model == null || type.IsInstanceOfType(model); ////model.GetType().IsAssignableFrom(type);
        }

        /// <summary>
        /// The get unsubscriber.
        /// </summary>
        /// <param name="observer">
        /// The observer.
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/>.
        /// </returns>
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
    }
}