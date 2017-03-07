namespace Merchello.Core
{
    using System;

    /// <summary>
    /// Warehouse constants.
    /// </summary>
    public static partial class Constants
    {
        /// <summary>
        /// The default warehouse keys
        /// </summary>
        public static class Warehouse
        {
            /// <summary>
            /// Gets the default warehouse key.
            /// </summary>
            public static Guid DefaultWarehouseKey
            {
                get { return new Guid("268D4007-8853-455A-89F7-A28398843E5F"); }
            }

            /// <summary>
            /// Gets the default warehouse catalog key.
            /// </summary>
            public static Guid DefaultWarehouseCatalogKey
            {
                get { return new Guid("B25C2B00-578E-49B9-BEA2-BF3712053C63"); }
            }
        }
    }
}