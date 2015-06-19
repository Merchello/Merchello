namespace Merchello.Core.Chains.OfferConstraints
{
    using Merchello.Core.Marketing.Offer;

    /// <summary>
    /// Defines a OfferProcessorFactory.
    /// </summary>
    internal interface IOfferProcessorFactory
    {
        /// <summary>
        /// Builds the <see cref="IOfferProcessor"/>
        /// </summary>
        /// <param name="offer">
        /// The offer.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferProcessor"/>.
        /// </returns>
        IOfferProcessor Build(OfferBase offer);
    }
}