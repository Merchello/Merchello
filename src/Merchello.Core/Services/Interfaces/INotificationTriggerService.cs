using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the NotificationTriggerService
    /// </summary>
    internal interface INotificationTriggerService : IService
    {
        /// <summary>
        /// Returns a collection of all <see cref="INotificationTrigger"/>
        /// </summary>
        IEnumerable<INotificationTrigger> GetAll();
    }
}