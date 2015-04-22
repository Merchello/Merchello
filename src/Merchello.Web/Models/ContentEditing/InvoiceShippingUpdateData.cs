namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The invoice shipping update data.
    /// </summary>
    public class InvoiceShippingUpdateData
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        public AddressDisplay Address { get; set; }
    }
}