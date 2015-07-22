namespace Merchello.Core
{
    using Merchello.Core.Gateways.Notification.Monitors;
    using Merchello.Core.Models;

    /// <summary>
    /// The auto mapper mappings.
    /// </summary>
    internal static class AutoMapperMappings
    {
        /// <summary>
        /// The create mappings.
        /// </summary>
        public static void CreateMappings()
        {
            // Notification message
            AutoMapper.Mapper.CreateMap<INotificationMessage, NotificationMonitorMessage>();
        }    
    }
}