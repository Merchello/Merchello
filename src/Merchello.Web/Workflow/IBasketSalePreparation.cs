namespace Merchello.Web.Workflow
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Umbraco.Core;

    /// <summary>
    /// Marker interface for <see cref="IBasket"/> based checkouts
    /// </summary>
    public interface IBasketSalePreparation : ISalePreparationBase
    {
        //Attempt<IOfferResult<ILineItemContainer, ILineItem>> TryValidateOffer(string offerCode);

        //Attempt<IOfferResult<TConstraint, TAward>> TryValidateOffer<TConstraint, TAward>(string offerCode)
        //    where TConstraint : class
        //    where TAward : class;

        /// <summary>
        /// Attempts to apply an offer to the the checkout.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="autoAddOnSuccess">
        /// A value indicating whether or not the reward should be added to the <see cref="ILineItemContainer"/> on success
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<IOfferResult<ILineItemContainer, ILineItem>> TryAwardOffer(string offerCode, bool autoAddOnSuccess = true);

        /// <summary>
        /// Attempts to apply an offer to the the checkout.
        /// </summary>
        /// <param name="validateAgainst">
        /// The object to validate against
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<IOfferResult<TConstraint, TAward>> TryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
            where TConstraint : class 
            where TAward : class;
    }
}