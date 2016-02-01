namespace Merchello.Bazaar.Models.ViewModels
{
    using Merchello.Core.Models;

    using Umbraco.Core.Models;

    /// <summary>
    /// The receipt model.
    /// </summary>
    public partial class ReceiptModel : MasterModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiptModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public ReceiptModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the invoice.
        /// </summary>
        public InvoiceSummary InvoiceSummary { get; set; }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        public IAddress ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the billing address.
        /// </summary>
        public IAddress BillingAddress { get; set; }
    }
}