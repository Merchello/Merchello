namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The checkout confirmation model.
    /// </summary>
    public class CheckoutConfirmationModel : CheckoutModelBase
    {
        /// <summary>
        /// The receipt page.
        /// </summary>
        private IPublishedContent _receiptPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutConfirmationModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public CheckoutConfirmationModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the checkout confirmation form.
        /// </summary>
        public CheckoutConfirmationForm CheckoutConfirmationForm { get; set; }

        /// <summary>
        /// Gets or sets the apply coupon form.
        /// </summary>
        public ApplyCouponForm ApplyCouponForm { get; set; }

        /// <summary>
        /// Gets the receipt page.
        /// </summary>
        public IPublishedContent ReceiptPage
        {
            get
            {
                return _receiptPage ?? StorePage.Descendant("BazaarReceipt");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Bazaar is configured to resolve the payment collection forms.
        /// </summary>
        public bool ResolvePaymentForms { get; set; }
    }
}