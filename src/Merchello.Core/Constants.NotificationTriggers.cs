namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Constants for Notification triggers.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// The default notification keys.
        /// </summary>
        internal static class NotificationTriggers
        {
            /// <summary>
            /// The invoice notification trigger keys.
            /// </summary>
            internal static class Invoice
            {
                /// <summary>
                /// The status changed trigger keys
                /// </summary>
                public static class StatusChanged
                {
                    /// <summary>
                    /// Gets the invoice status changed to paid.
                    /// </summary>
                    public static Guid ToPaid
                    {
                        get { return new Guid("645E7E24-3F97-467D-9923-0CC4DD96468C"); }
                    }

                    /// <summary>
                    /// Gets invoice status changed the to partial.
                    /// </summary>
                    public static Guid ToPartial
                    {
                        get { return new Guid("5AF5E106-697E-438E-B768-876666C4E333"); }
                    }

                    /// <summary>
                    /// Gets the to cancelled.
                    /// </summary>
                    public static Guid ToCancelled
                    {
                        get { return new Guid("BB2AC8F1-B813-41BE-BFEE-F5A95CBB541A"); }
                    }
                }
            }

            /// <summary>
            /// The order triggers.
            /// </summary>
            internal static class Order
            {
                /// <summary>
                /// The status changed notification triggers
                /// </summary>
                public static class StatusChanged
                {
                    /// <summary>
                    /// Gets the to back order notification trigger key
                    /// </summary>
                    public static Guid ToBackOrder
                    {
                        get { return new Guid("7E2858CB-B41C-456F-9D25-EED36FCD4FBC"); }
                    }

                    /// <summary>
                    /// Gets the status changed to fulfilled notification trigger key
                    /// </summary>
                    public static Guid ToFulfilled
                    {
                        get { return new Guid("39D15CDF-32B6-41CC-95F0-483A676F90C0"); }
                    }

                    /// <summary>
                    /// Gets the to cancelled.
                    /// </summary>
                    public static Guid ToCancelled
                    {
                        get { return new Guid("6B495772-D1E2-4515-955C-1E65C86C23D4"); }
                    }
                }
            }
        }
    }
}