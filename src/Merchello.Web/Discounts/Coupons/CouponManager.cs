namespace Merchello.Web.Discounts.Coupons
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;
    using Merchello.Web.Trees;

    /// <summary>
    /// The provider responsible for managing coupon offers
    /// </summary>
    [BackOfficeTree("coupons", "marketing", "Coupon", "icon-receipt-alt", "merchello/merchello/couponeditor/{0}", 1)]
    public class CouponManager : OfferProviderBase<Coupon>
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