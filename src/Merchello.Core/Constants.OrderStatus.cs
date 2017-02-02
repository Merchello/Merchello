namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Order Status Constants.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// The order status keys
        /// </summary>
        public static class OrderStatus
        {
            /// <summary>
            /// Gets the not fulfilled (or not shipped) order status key
            /// </summary>
            public static Guid NotFulfilled
            {
                get { return new Guid("C54D47E6-D1C9-40D5-9BAF-18C6ADFFE9D0"); }
            }

            /// <summary>
            /// Gets the order status open key.
            /// </summary>
            public static Guid Open
            {
                get { return new Guid("E67B414E-0E55-480D-9429-1204AD46ECDA"); }
            }

            /// <summary>
            /// Gets the back order "back ordered" order status key
            /// </summary>
            public static Guid BackOrder
            {
                get { return new Guid("C47D475F-A075-4635-BBB9-4B9C49AA8EBE"); }
            }

            /// <summary>
            /// Gets the fulfilled (or completed) order status key.
            /// </summary>
            public static Guid Fulfilled
            {
                get { return new Guid("D5369B84-8CCA-4586-8FBA-F3020F5E06EC"); }
            }

            /// <summary>
            /// Gets the cancelled order status key.
            /// </summary>
            public static Guid Cancelled
            {
                get { return new Guid("77DAF52E-C79C-4E1B-898C-5E977A9A6027"); }
            }
        }
    }
}