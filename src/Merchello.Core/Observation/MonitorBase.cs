using System;
using Umbraco.Core.Logging;

namespace Merchello.Core.Observation
{
    /// <summary>
    /// Defines a Base Monitor
    /// </summary>
    public abstract class MonitorBase<T> : IObserver<T>
    {
        #region IObserver<T> implementation

        /// <summary>
        /// Performs the action
        /// </summary>
        /// <param name="value"></param>
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
    }
}