namespace Merchello.FastTrack.Models.Payment
{
    using Merchello.Web.Store.Models;

    /// <summary>
    /// A model for rendering and processing basic BrainTree Payments.
    /// </summary>
    public class FastTrackBraintreePaymentModel : BraintreePaymentModel, ISuccessRedirectUrl
    {
        /// <summary>
        /// Gets or sets the success redirect url.
        /// </summary>
        public string SuccessRedirectUrl { get; set; }
    }
}