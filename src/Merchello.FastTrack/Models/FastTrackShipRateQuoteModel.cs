namespace Merchello.FastTrack.Models
{
    using Merchello.Web.Models.Ui;
    using Merchello.Web.Store.Models;

    /// <summary>
    /// A model for FastTrack ship rate quotes.
    /// </summary>
    public class FastTrackShipRateQuoteModel : StoreShipRateQuoteModel, ISuccessRedirectUrl
    {
        /// <summary>
        /// Gets or sets the success URL to redirect to the shipping entry stage.
        /// </summary>
        public string SuccessRedirectUrl { get; set; }
    }
}