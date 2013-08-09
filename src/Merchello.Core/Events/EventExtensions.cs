using System;

namespace Merchello.Core.Events
{
    public static class EventExtensions
    {
        /// <summary>
        /// Raises the event
        /// </summary>
        /// <typeparam name="TSender"></typeparam>
        /// <typeparam name="TArgs"></typeparam>
        /// <param name="eventHandler"></param>
        /// <param name="args"></param>
        /// <param name="sender"></param>
        public static void RaiseEvent<TSender, TArgs>(
            this TypedEventHandler<TSender, TArgs> eventHandler,
            TArgs args,
            TSender sender)
        {
            if (eventHandler != null) eventHandler(sender, args);
        }
    }
}
