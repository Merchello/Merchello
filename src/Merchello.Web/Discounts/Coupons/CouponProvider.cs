namespace Merchello.Web.Discounts.Coupons
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Services;

    public class CouponProvider : OfferProviderBase<Coupon>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponProvider"/> class.
        /// </summary>
        /// <param name="offerSettingsService">
        /// The offer settings service.
        /// </param>
        public CouponProvider(IOfferSettingsService offerSettingsService)
            : base(offerSettingsService)
        {
        }

        public override Guid Key
        {
            get
            {
                return new Guid("1EED2CCB-4146-44BE-A5EB-DA3D2E3992A7");
            }
        }

        protected override Coupon GetInstance(IOfferSettings offerSettings)
        {
            return new Coupon(offerSettings);
        }
    }
}