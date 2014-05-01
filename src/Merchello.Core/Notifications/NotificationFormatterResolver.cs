using System.Collections.Generic;
using Umbraco.Core.ObjectResolution;

namespace Merchello.Core.Notifications
{
    /// <summary>
    /// Resolves Notification Formatters
    /// </summary>
    internal sealed class NotificationFormatterResolver : ManyObjectsResolverBase<NotificationFormatterResolver, INotificationFormatter>
    {
        /// <summary>
        /// Gets the finders.
        /// </summary>
        public IEnumerable<INotificationFormatter> Formatters
        {
            get { return Values; }
        }
    }
}