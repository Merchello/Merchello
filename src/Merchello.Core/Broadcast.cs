using System;
using Merchello.Core.Events;

namespace Merchello.Core
{
    /// <summary>
    /// Broadcasts a message 
    /// </summary>
    public class Broadcast<T>
        where T : EventArgs
    {

        public event EventHandler<T> Message;

        protected virtual void OnAnnouncement(T args)
        {
            if (Message != null)
            {
                Message(this, args);
            }
        }
    }


    public class Broadcast
    {


        public static event EventHandler<NotificationEventArgs> Notification;

        public static void OnNotification(NotificationEventArgs args)
        {
            if (Notification != null) Notification(null, args);
        }
    }


}