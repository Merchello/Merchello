namespace Merchello.Web.Models.Ui
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Models;

    /// <summary>
    /// Defines an address used in checkout implementations.
    /// </summary>
    /// <remarks>
    /// This interface asserts the address collected can be used in invoices, shipments and be persisted
    /// as a <see cref="ICustomerAddress"/>
    /// </remarks>
    public interface ICheckoutAddressModel : ICheckoutModel, IAddress
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        /// <remarks>
        /// This the Merchello customer address key
        /// </remarks>
        Guid Key { get; set; }

        /// <summary>
        /// Gets or sets a label for the address (e.g. My House).
        /// </summary>
        string Label { get; set; }
    }
}