namespace Merchello.Core.Observation
{
    using System;
    using System.Reflection;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Defines a Base Monitor
    /// </summary>
    /// <typeparam name="T">
    /// The type of the monitor Model
    /// </typeparam>
    public abstract class MonitorBase<T> : IObserver<T>, IMonitor
    {
        /// <summary>
        /// Gets the obverves type of the monitor
        /// </summary>
        public Type ObservesType
        {
            get { return typeof(T); }
        }

        #region IObserver<T> implementation

        /// <summary>
        /// Performs the action
        /// </summary>
        /// <param name="value">The model used in the monitor</param>
        public abstract void OnNext(T value);

        public virtual void OnError(Exception error)
        {
            LogHelper.Error<MonitorBase<T>>("Monitor error: ", error);
        }

        public virtual void OnCompleted()
        {
            LogHelper.Debug<MonitorBase<T>>(string.Format("Completed monitoring {0}", GetType()));
        }

        #endregion

        /// <summary>
        /// Subscribes itself to a <see cref="ITrigger"/>
        /// </summary>
        /// <param name="resolver">
        /// The <see cref="ITriggerResolver"/> that resolves the trigger this monitor subscribes to
        /// </param>
        /// <returns>
        /// The <see cref="IDisposable"/> monitor
        /// </returns>
        public IDisposable Subscribe(ITriggerResolver resolver)
        {
            var att = GetType().GetCustomAttribute<MonitorForAttribute>(false);
            if (att != null)
            {
                var trigger = (IObservable<T>)resolver.GetTrigger(att.ObservableTrigger);
                if (trigger != null)
                {
                    LogHelper.Info<MonitorBase<T>>(string.Format("{0} subscribing to {1}", GetType().Name, trigger.GetType().Name));
                    return trigger.Subscribe(this);                    
                }
            }

            LogHelper.Debug<MonitorBase<T>>("Subscribe failed for type" + GetType());
            return null;
        }
    }
}