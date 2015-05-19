namespace Merchello.Core.Marketing.Offer
{
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// A base for Offer classes
    /// </summary>
    public abstract class OfferBase : IOffer 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferBase"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IOfferSettings"/>.
        /// </param>
        protected OfferBase(IOfferSettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");

            this.Settings = settings;
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        protected IOfferSettings Settings { get; private set; }

    }
}