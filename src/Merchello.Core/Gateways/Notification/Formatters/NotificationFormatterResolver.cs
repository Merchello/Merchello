using System;
using System.Collections.Generic;
using Merchello.Core.ObjectResolution;

namespace Merchello.Core.Gateways.Notification.Formatters
{
    /// <summary>
    /// Resolves Notification Formatters
    /// </summary>
    internal sealed class NotificationFormatterResolver : MerchelloManyObjectsResolverBase<NotificationFormatterResolver, INotificationFormatter> 
    {
        public NotificationFormatterResolver(IEnumerable<Type> value) : base(value)
        { }

        /// <summary>
        /// Gets the finders.
        /// </summary>
        public IEnumerable<INotificationFormatter> Formatters
        {
            get { return Values; }
        }

        protected override IEnumerable<INotificationFormatter> Values
        {
            get { throw new NotImplementedException(); }
        }
    }
}