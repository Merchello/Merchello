namespace Merchello.Core
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Gateways.Notification.Triggering;
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
        /// <param name="alias">The alias of the trigger</param>
        public static void Trigger(string alias)
        {
            Trigger(alias, null);
        }

        /// <summary>
        /// Triggers a notification by it's alias and includes a model to pass information to the message
        /// </summary>
        /// <param name="alias">The alias of the trigger</param>
        /// <param name="model">The model to be passed to the monitor</param>
        public static void Trigger(string alias, object model)
        {
            Trigger(alias, model, new string[] { });
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
        public static void Trigger(string alias, object model, IEnumerable<string> contacts)
        {
            var contactsArray = contacts as string[] ?? contacts.ToArray();

            var triggers = GetTrigger(alias).OfType<INotificationTrigger>();

            foreach (var notificationTrigger in triggers)
            {                
                notificationTrigger.Notify(model, contactsArray);
            }
        }

        /// <summary>
        /// Utility method used to get a collection of triggers from the resolver that match the alias passed
        /// </summary>
        /// <param name="alias">The alias to match when resolving the triggers</param>
        /// <returns>A collection of triggers</returns>
        private static IEnumerable<ITrigger> GetTrigger(string alias)
        {
            if (TriggerResolver.HasCurrent) return TriggerResolver.Current.GetTriggersByAlias(alias);

            var ex = new InvalidOperationException("TriggerResolver.Current has not been initialized.");

            LogHelper.Error(typeof(Notification), "TriggerResolver has not be initialized", ex);
            
            throw ex;
        }
    }
}