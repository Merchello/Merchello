namespace Merchello.FastTrack.Models.Payment
{
    using Merchello.Web.Store.Models;

    /// <summary>
    /// The cash payment model for the FastTrack store.
    /// </summary>
    public class FastTrackPaymentModel : StorePaymentModel, ISuccessRedirectUrl
    {
        /// <summary>
        /// Gets or sets the success redirect url.
        /// </summary>
        public string SuccessRedirectUrl { get; set; }
    }
}