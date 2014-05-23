using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Observation;

namespace Merchello.Core.Gateways.Notification.Triggering
{
    [ObservationChannelFor("ACFB0BA8-9407-4438-AA37-973E7A700368", "Order Confirmation", "OrderConfirmation", ObservationChannelType.Notification)]
    public class OrderConfirmationChannel : NotificationObservationChannelBase<IInvoice>
    {
        public override void Update(object model, IEnumerable<string> contacts)
        {
            throw new NotImplementedException();
        }
    }
}