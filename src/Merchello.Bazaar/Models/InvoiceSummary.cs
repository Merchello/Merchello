namespace Merchello.Bazaar.Models
{
    using Merchello.Core.Models;

    /// <summary>
    /// A model for the InvoiceSummary partial view.
    /// </summary>
    public partial class InvoiceSummary
    {
        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        public IInvoice Invoice { get; set; }

        /// <summary>
        /// Gets or sets the currency.
        /// </summary>
        public ICurrency Currency { get; set; }

        /// <summary>
        /// Gets or sets the current page id.
        /// </summary>
        public int CurrentPageId { get; set; }
    }
}