using System;

namespace Merchello.Core.Gateways.Notification
{
    internal sealed class Notify
    {
        #region SingleTon

        private static readonly Lazy<CustomerAnnouncement> Lazy = new Lazy<CustomerAnnouncement>(() => new CustomerAnnouncement());

        public static CustomerAnnouncement Customer { get { return Lazy.Value; } }

        #endregion
        
    }
}