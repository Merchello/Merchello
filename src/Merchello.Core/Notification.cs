﻿namespace Merchello.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Gateways.Notification.Triggering;

    using Merchello.Core.Logging;

    using Observation;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Utility class used to trigger notifications inline
    /// </summary>
    public static class Notification
    {
        /// <summary>
        /// Triggers a notification by it's alias
        /// </summary>
        /// <param name="alias">
        /// The alias of the trigger
        /// </param>
        /// <param name="topic">
        /// The trigger topic.
        /// </param>
        public static void Trigger(string alias, Topic topic = Topic.Notifications)
        {
            Trigger(alias, null, topic);
        }

        /// <summary>
        /// Triggers a notification by it's alias and includes a model to pass information to the message
        /// </summary>
        /// <param name="alias">
        /// The alias of the trigger
        /// </param>
        /// <param name="model">
        /// The model to be passed to the monitor
        /// </param>
        /// <param name="topic">
        /// The topic.
        /// </param>
        public static void Trigger(string alias, object model, Topic topic = Topic.Notifications)
        {
            Trigger(alias, model, new string[] { }, topic);
        }

        /// <summary>
        /// Triggers a notification by it's alias and includes a model to pass information to the message
        /// </summary>
        /// <param name="alias">
        /// The alias of the trigger
        /// </param>
        /// <param name="model">
        /// The model to be passed to the monitor
        /// </param>
        /// <param name="contacts">
        /// An additional list of contacts
        /// </param>
        /// <param name="topic">
        /// The topic.
        /// </param>
        public static void Trigger(string alias, object model, IEnumerable<string> contacts, Topic topic = Topic.Notifications)
        {
            var contactsArray = contacts as string[] ?? contacts.ToArray();

            var triggers = GetTrigger(alias, topic);

            foreach (var notificationTrigger in triggers.OfType<INotificationTrigger>())
            {                
                notificationTrigger.Notify(model, contactsArray);
            }
        }

        /// <summary>
        /// Utility method used to get a collection of triggers from the resolver that match the alias passed
        /// </summary>
        /// <param name="alias">
        /// The alias to match when resolving the triggers
        /// </param>
        /// <param name="topic">
        /// The topic.
        /// </param>
        /// <returns>
        /// A collection of triggers
        /// </returns>
        private static IEnumerable<ITrigger> GetTrigger(string alias, Topic topic = Topic.Notifications)
        {
            if (TriggerResolver.HasCurrent) return TriggerResolver.Current.GetTriggersByAlias(alias, topic);

            var ex = new InvalidOperationException("TriggerResolver.Current has not been initialized.");

            MultiLogHelper.Error(typeof(Notification), "TriggerResolver has not be initialized", ex);
            
            throw ex;
        }
    }
}