namespace Merchello.Web.Discounts.Coupons
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Web.Trees;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    /// <summary>
    /// The provider responsible for managing coupon offers
    /// </summary>
    [BackOfficeTree("coupons", "marketing", "Coupon", "icon-receipt-alt", "merchello/merchello/couponeditor/{0}", 1)]
    public class CouponManager : OfferManagerBase<Coupon>
    {
        /// <summary>
        /// The _instance.
        /// </summary>
        private static CouponManager _instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponManager"/> class.
        /// </summary>
        public CouponManager()
            : this(MerchelloContext.Current.Services.OfferSettingsService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponManager"/> class.
        /// </summary>
        /// <param name="offerSettingsService">
        /// The offer settings service.
        /// </param>
        public CouponManager(IOfferSettingsService offerSettingsService)
            : base(offerSettingsService)
        {
        }

        #region Events

        /// <summary>
        /// Occurs before redeeming the coupon
        /// </summary>
        public static event TypedEventHandler<CouponManager, RedeemCouponEventArgs> Redeeming;

        /// <summary>
        /// Occurs after redeeming the coupon.
        /// </summary>
        public static event TypedEventHandler<CouponManager, RedeemCouponEventArgs> Redeemed;

        #endregion

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static CouponManager Instance
        {
            get
            {
                return _instance ?? new CouponManager();
            }
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public override Guid Key
        {
            get
            {
                return new Guid("1EED2CCB-4146-44BE-A5EB-DA3D2E3992A7");
            }
        }

        /// <summary>
        /// Gets an offer by it's offer code (with manager defaults).
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<Coupon> GetByOfferCode(string offerCode, ICustomerBase customer)
        {
            return GetByOfferCode<ILineItemContainer, ILineItem>(offerCode, customer);
        }

        /// <summary>
        /// The safe add coupon attempt container.
        /// </summary>
        /// <typeparam name="TLineItem">
        /// The type of line item
        /// </typeparam>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional parameter indicating whether or not to raise events.  Defaults to false.
        /// </param>
        internal void SafeAddCouponAttemptContainer<TLineItem>(ILineItemContainer container, ICouponRedemptionResult result, bool raiseEvents = false)
            where TLineItem : class, ILineItem
        {
            if (!result.Success) return;

            // the award is the line item
            var lineItem = result.Award;

            // TODO if there is to be line items of types other than discount line items, this is where they should be added.

            if (container.Items.Contains(lineItem.Sku)) return;
            lineItem.ExtendedData.SetCouponValue(result.Coupon);

            if (raiseEvents)
            Redeeming.RaiseEvent(new RedeemCouponEventArgs(container, lineItem), this);
            
            container.Items.Add(lineItem.AsLineItemOf<TLineItem>());

            if (raiseEvents)
            Redeemed.RaiseEvent(new RedeemCouponEventArgs(container, lineItem), this);
        }

        /// <summary>
        /// Gets an instance of a coupon from the <see cref="IOfferSettings"/>        
        /// /// </summary>
        /// <param name="offerSettings">
        /// The offer settings.
        /// </param>
        /// <returns>
        /// The <see cref="Coupon"/>.
        /// </returns>
        protected override Coupon GetInstance(IOfferSettings offerSettings)
        {
            return new Coupon(offerSettings);
        }

    }
}