namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;
    using Umbraco.Web;

    /// <summary>
    /// The checkout model.
    /// </summary>
    public partial class CheckoutModel : CheckoutModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public CheckoutModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the checkout address form.
        /// </summary>
        public CheckoutAddressForm CheckoutAddressForm { get; set; }

        /// <summary>
        /// Gets the continue checkout page
        /// </summary>
        public IPublishedContent ContinueCheckoutPage 
        {
            get
            {
                return Content.Descendant("BazaarCheckoutConfirm");
            } 
        }
    }
}