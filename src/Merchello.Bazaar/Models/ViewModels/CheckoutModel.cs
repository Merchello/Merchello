namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The checkout model.
    /// </summary>
    public class CheckoutModel : CheckoutModelBase
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
        /// Gets or sets the billing address.
        /// </summary>
        public AddressFormModel BillingAddress { get; set; }
    }
}