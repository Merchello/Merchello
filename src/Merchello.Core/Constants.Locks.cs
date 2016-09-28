namespace Merchello.Core
{
    /// <summary>
    /// Constants for service locks.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// Defines locks (used in services).
        /// </summary>
        public static class Locks
        {
            /// <summary>
            /// Lock for settings.
            /// </summary>
            public const int Settings = -911;

            /// <summary>
            /// Lock for products.
            /// </summary>
            public const int ProductTree = -921;

            /// <summary>
            /// Lock for sales.
            /// </summary>
            public const int SalesTree = -931;

            /// <summary>
            /// Lock for shipments.
            /// </summary>
            public const int Shipments = -932;

            /// <summary>
            /// Lock for customers.
            /// </summary>
            public const int CustomersTree = -941;

            /// <summary>
            /// Lock for marketing.
            /// </summary>
            public const int MarketingTree = -951;

            /// <summary>
            /// Lock for providers.
            /// </summary>
            public const int ProvidersTree = -961;
        }
    }
}