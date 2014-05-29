using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Gateways.Notification.Triggering;
using Merchello.Core.Observation;
using Umbraco.Core.Logging;

namespace Merchello.Core
{
    public class Notification
    {

        public static void Trigger(string alias)
        {
            Trigger(alias, null);
        }

        public static void Trigger(string alias, object model)
        {
            Trigger(alias, model, new string[]{});
        }

        public static void Trigger(string alias, object model, IEnumerable<string> contacts)
        {
            foreach (var notificationTrigger in GetTrigger(alias).OfType<INotificationTrigger>())
            {
                notificationTrigger.Notify(model, contacts);
            }
        }


        private static IEnumerable<ITrigger> GetTrigger(string alias)
        {
            if (TriggerResolver.HasCurrent) return TriggerResolver.Current.GetTriggersByAlias(alias);

            var ex = new InvalidOperationException("TriggerResolver.Current has not been initialized.");
            LogHelper.Error<Notification>("TriggerResolver has not be initialized", ex);
            throw ex;
        }
    }

}