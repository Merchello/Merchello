using System;

namespace Merchello.Core.Broadcast
{
    /// <summary>
    /// Broadcasts a message 
    /// </summary>
    public abstract class BroadcasterBase<T>
        where T : EventArgs
    {

        public static event EventHandler<T> Broadcasting;

        protected virtual void OnBroadcasting(T args)
        {
            if (Broadcasting != null)
            {
                Broadcasting(this, args);
            }
        }
    }
}