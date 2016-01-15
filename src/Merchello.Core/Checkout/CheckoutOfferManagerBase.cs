namespace Merchello.Core.Checkout
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Sales;

    using Newtonsoft.Json;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// Represents a checkout offer manager.
    /// </summary>
    public abstract class CheckoutOfferManagerBase : CheckoutCustomerDataManagerBase, ICheckoutOfferManager
    {
        /// <summary>
        /// The offer code temp data.
        /// </summary>
        private Lazy<List<string>> _offerCodeTempData;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckoutOfferManagerBase"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        protected CheckoutOfferManagerBase(ICheckoutContext context)
          : base(context)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets the offer codes.
        /// </summary>
        public IEnumerable<string> OfferCodes
        {
            get
            {
                return this._offerCodeTempData.Value;
            }
        }       

        /// <summary>
        /// Removes an offer code from the OfferCodes collection.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        public virtual void RemoveOfferCode(string offerCode)
        {
            if (OfferCodes.Contains(offerCode))
            {
                _offerCodeTempData.Value.Remove(offerCode);
                this.SaveCustomerTempData(Core.Constants.ExtendedDataKeys.OfferCodeTempData, this._offerCodeTempData.Value);
            }
        }

        /// <summary>
        /// Clears the offer codes collection.
        /// </summary>
        public void ClearOfferCodes()
        {
            _offerCodeTempData.Value.Clear();
            this.SaveCustomerTempData(Core.Constants.ExtendedDataKeys.OfferCodeTempData, this._offerCodeTempData.Value);
        }

        /// <summary>
        /// Attempts to redeem an offer to the sale.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedemptionResult{ILineItem}"/>.
        /// </returns>
        public abstract IOfferRedemptionResult<ILineItem> RedeemCouponOffer(string offerCode);

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
        /// TODO RSS internal abstract will make it impossible for people to write their own CheckoutOfferManagers
        /// </remarks>
        internal abstract Attempt<IOfferResult<TConstraint, TAward>> TryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
            where TConstraint : class
            where TAward : class;

        /// <summary>
        /// Saves offer code.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected bool SaveOfferCode(string offerCode)
        {
            if (!OfferCodes.Contains(offerCode))
            {
                _offerCodeTempData.Value.Add(offerCode);
                this.SaveCustomerTempData(Core.Constants.ExtendedDataKeys.OfferCodeTempData, this._offerCodeTempData.Value);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Initializes the manager.
        /// </summary>
        private void Initialize()
        {
            this._offerCodeTempData = new Lazy<List<string>>(() => BuildVersionedCustomerTempData(Core.Constants.ExtendedDataKeys.OfferCodeTempData));

            if (Context.IsNewVersion && Context.Settings.ResetOfferManagerDataOnVersionChange) this.ClearOfferCodes();
        }
    }
}