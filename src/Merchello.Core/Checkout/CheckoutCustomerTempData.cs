namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Utility class for saving and retrieving serialized strings arrays to customer's ExtendedDataCollection by checkout version.
    /// </summary>
    public class CheckoutCustomerTempData
    {
        /// <summary>
        /// Gets or sets the version key to validate offer codes are validate with this preparation
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the enumerable string of data.
        /// </summary>
        public IEnumerable<string> Data { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the reset was cancelled.
        /// </summary>
        public bool ResetWasCancelled { get; set; }
    }
}