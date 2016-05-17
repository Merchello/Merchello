namespace Merchello.Web.Store.Models
{
    /// <summary>
    /// A model for rendering and processing basic BrainTree Payments.
    /// </summary>
    public class BraintreePaymentModel : StorePaymentModel
    {
        /// <summary>
        /// Gets or sets the server token.
        /// </summary>
        public string Token { get; set; }
    }
}