namespace Merchello.Bazaar.Models.ViewModels
{
    using Umbraco.Core.Models;

    /// <summary>
    /// The checkout shipping model.
    /// </summary>
    public class CheckoutShippingModel : CheckoutModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutShippingModel"/> class.
        /// </summary>
        /// <param name="content">
        /// The content.
        /// </param>
        public CheckoutShippingModel(IPublishedContent content)
            : base(content)
        {
        }

        /// <summary>
        /// Gets or sets the shipping address.
        /// </summary>
        public AddressFormModel ShippingAddress { get; set; }
    }
}