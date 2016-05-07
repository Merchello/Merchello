namespace Merchello.Implementation.Models.Async
{
    using System;

    /// <summary>
    /// Represents an updated line item.
    /// </summary>
    internal class UpdateQuantityResponseItem
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the quantity.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the formatted total.
        /// </summary>
        public string FormattedTotal { get; set; }
    }
}