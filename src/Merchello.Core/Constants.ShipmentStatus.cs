namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Shipment status constants.
    /// </summary>
    public partial class Constants
    {
        /// <summary>
        /// The shipment status.
        /// </summary>
        public static class ShipmentStatus
        {
            /// <summary>
            /// Gets the quoted shipment status key
            /// </summary>
            public static Guid Quoted
            {
                get { return new Guid("6FA425A9-7802-4DA0-BD33-083C100E30F3"); }
            }

            /// <summary>
            /// Gets the packaging status key.
            /// </summary>
            public static Guid Packaging
            {
                get { return new Guid("7342DCD6-8113-44B6-BFD0-4555B82F9503"); }
            }

            /// <summary>
            /// Gets the shipment ready status key.
            /// </summary>
            public static Guid Ready
            {
                get { return new Guid("CB24D43F-2774-4E56-85D8-653E49E3F542"); }
            }

            /// <summary>
            /// Gets the shipment shipped status key.
            /// </summary>
            public static Guid Shipped
            {
                get { return new Guid("B37BE101-CEC9-4608-9330-54E56FA0537A"); }
            }

            /// <summary>
            /// Gets the delivered shipment status key.
            /// </summary>
            public static Guid Delivered
            {
                get { return new Guid("3A279633-4919-485D-8C3B-479848A053D9"); }
            }
        }
    }
}