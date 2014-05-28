using System;
using System.Collections.Generic;
using Merchello.Core.Models;

namespace Merchello.Core.Gateways.Notification
{
    /// <summary>
    /// Defines the NotificationContext
    /// </summary>
    public interface INotificationContext : IGatewayProviderTypedContextBase<NotificationGatewayProviderBase>
    {
        ///// <summary>
        ///// Gets a collection of <see cref="INotificationMessage"/>s by a Monitor Key (Guid)
        ///// </summary>
        ///// <param name="monitorKey"></param>
        ///// <returns>A collection of NotificationMessage</returns>
        //IEnumerable<INotificationMessage> GetNotificationMessagesByMonitorKey(Guid monitorKey);
    }
}