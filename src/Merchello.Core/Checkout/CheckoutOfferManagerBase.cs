namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Sales;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a checkout offer manager.
    /// </summary>
    public abstract class CheckoutOfferManagerBase : CheckoutContextManagerBase, ICheckoutOfferManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutOfferManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutOfferManagerBase(ICheckoutContext context)
          : base(context)
        {
        }

        /// <summary>
        /// The build offer code list.
        /// </summary>
        /// <returns>
        /// The <see cref="List{String}"/>.
        /// </returns>
        public virtual List<string> BuildOfferCodeList()
        {

            var codes = new List<string>();
            var queueDataJson = Context.Customer.ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.OfferCodeTempData);
            if (string.IsNullOrEmpty(queueDataJson)) return codes;

            try
            {
                var savedData = JsonConvert.DeserializeObject<OfferCodeTempData>(queueDataJson);

                // verify that the offer codes are for this version of the checkout
                if (savedData.VersionKey != Context.VersionKey) return codes;

                codes.AddRange(savedData.OfferCodes);
            }
            catch (Exception ex)
            {
                // don't throw an exception here as the customer is in the middle of a checkout.
                LogHelper.Error<SalePreparationBase>("Failed to deserialize OfferCodeTempData.  Returned empty offer code list instead.", ex);
            }

            return codes;
        }

        public virtual void RemoveOfferCode(string offerCode)
        {
            throw new System.NotImplementedException();
        }

        public void ClearOfferCodes()
        {
            throw new System.NotImplementedException();
        }


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
        /// <remarks>
        /// Custom offer types
        /// </remarks>
        internal abstract Attempt<IOfferResult<TConstraint, TAward>> TryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
            where TConstraint : class
            where TAward : class;


        /// <summary>
        /// Class that gets serialized to customer's ExtendedDataCollection to save offer code queue data.
        /// </summary>
        protected struct OfferCodeTempData
        {
            /// <summary>
            /// Gets or sets the version key to validate offer codes are validate with this preparation
            /// </summary>
            public Guid VersionKey { get; set; }

            /// <summary>
            /// Gets or sets the offer codes.
            /// </summary>
            public IEnumerable<string> OfferCodes { get; set; }
        }
    }
}