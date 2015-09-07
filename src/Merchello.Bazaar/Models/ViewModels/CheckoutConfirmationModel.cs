namespace Merchello.Bazaar.Models.ViewModels
{
    using System;
    using System.ComponentModel;

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
        private Lazy<IPublishedContent> _receiptPage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutConfirmationModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public CheckoutConfirmationModel(IPublishedContent content)
            : base(content)
        {
            Initialize();
        }

        /// <summary>
        /// Gets or sets the checkout confirmation form.
        /// </summary>
        public CheckoutConfirmationForm CheckoutConfirmationForm { get; set; }

        /// <summary>
        /// Gets or sets the apply coupon form.
        /// </summary>
        public RedeemCouponOfferForm RedeemCouponOfferForm { get; set; }

        /// <summary>
        /// Gets the receipt page.
        /// </summary>
        public IPublishedContent ReceiptPage
        {
            get
            {
                return _receiptPage.Value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Bazaar is configured to resolve the payment collection forms.
        /// </summary>
        public bool ResolvePaymentForms { get; set; }

        /// <summary>
        /// Initializes the model.
        /// </summary>
        private void Initialize()
        {
            _receiptPage = new Lazy<IPublishedContent>(() => BazaarContentHelper.GetStoreRoot().Descendant("BazaarReceipt"));
        }
    }
}