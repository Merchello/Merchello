namespace Merchello.Core.Marketing.Rewards
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a discount line item reward.
    /// </summary>
    [OfferComponent("A1CCE36A-C5AA-4C50-B659-CC2FBDEAA7B3", "Discount the price", "Applies a discount according to configured price rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerreward.discountprice.html")]
    public class DiscountLineItemReward : OfferRewardComponentBase<ILineItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DiscountLineItemReward"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        public DiscountLineItemReward(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Attempts to construct the discount line item reward to be awarded.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<ILineItem> Award(ICustomerBase customer, ILineItemContainer container)
        {
            throw new System.NotImplementedException();
        }
    }
}