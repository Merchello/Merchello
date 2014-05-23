using System;
using Umbraco.Core.Logging;

namespace Merchello.Core.Observation
{
    public abstract class MonitorBase<T> : IObserver<T>
    {
        /// <summary>
        /// Performs the action
        /// </summary>
        /// <param name="value"></param>
        public abstract void OnNext(T value);
        

        public void OnError(Exception error)
        {
            LogHelper.Error<MonitorBase<T>>("Monitor error: ", error);
        }

        public void OnCompleted()
        {
            LogHelper.Debug<MonitorBase<T>>(string.Format("Completed monitoring {0}", GetType()));
        }
    }
}