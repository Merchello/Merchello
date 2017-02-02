namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Invoice status constants.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// The default invoice status keys.
        /// </summary>
        public static class InvoiceStatus
        {
            /// <summary>
            /// Gets the unpaid invoice status key.
            /// </summary>
            public static Guid Unpaid
            {
                get { return new Guid("17ADA9AC-C893-4C26-AA26-234ECEB2FA75"); }
            }

            /// <summary>
            /// Gets the paid invoice status key.
            /// </summary>
            public static Guid Paid
            {
                get { return new Guid("1F872A1A-F0DD-4C3E-80AB-99799A28606E"); }
            }

            /// <summary>
            /// Gets the partially paid invoice status key.
            /// </summary>
            public static Guid Partial
            {
                get { return new Guid("6606B0EA-15B6-44AA-8557-B2D9D049645C"); }
            }

            /// <summary>
            /// Gets the cancelled invoice status key.
            /// </summary>
            public static Guid Cancelled
            {
                get { return new Guid("53077EFD-6BF0-460D-9565-0E00567B5176"); }
            }

            /// <summary>
            /// Gets the fraud invoice status key
            /// </summary>
            public static Guid Fraud
            {
                get { return new Guid("75E1E5EB-33E8-4904-A8E5-4B64A37D6087"); }
            }
        }
    }
}