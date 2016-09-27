namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Distributed cache refreshers
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Cache refresher keys.
        /// </summary>
        public static class DistributedCache
        {
            /// <summary>
            /// Gets the PaymentCacheRefresher key.
            /// </summary>
            public static Guid PaymentCacheRefresherKey
            {
                get
                {
                    return new Guid("1ED410AB-AB61-48CF-8F52-37C92B4213A1");
                }
            }
        }
    }
}