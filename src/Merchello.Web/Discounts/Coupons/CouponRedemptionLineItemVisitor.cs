namespace Merchello.Web.Discounts.Coupons
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Web.Models.ContentEditing;

    using Newtonsoft.Json;

    /// <summary>
    /// A visitor to assist in recording coupon offer redemptions.
    /// </summary>
    internal class CouponRedemptionLineItemVisitor : ILineItemVisitor
    {
        /// <summary>
        /// The list of coupons in the invoice
        /// </summary>
        private readonly List<IOfferRedeemed> _redemptions = new List<IOfferRedeemed>();

        /// <summary>
        /// The _customer key.
        /// </summary>
        private readonly Guid? _customerKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponRedemptionLineItemVisitor"/> class.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        public CouponRedemptionLineItemVisitor(Guid? customerKey)
        {
            _customerKey = customerKey;
        }

        /// <summary>
        /// Gets the coupon redemptions.
        /// </summary>
        public IEnumerable<IOfferRedeemed> Redemptions
        {
            get
            {
                return _redemptions;
            }
        } 

        /// <summary>
        /// The visit.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        public void Visit(ILineItem lineItem)
        {
            if (lineItem.LineItemType != LineItemType.Discount) return;
            if (!lineItem.ExtendedData.ContainsKey(Core.Constants.ExtendedDataKeys.CouponReward)) return;

            var json = lineItem.ExtendedData.GetValue(Core.Constants.ExtendedDataKeys.CouponReward);
            var offerSettings = JsonConvert.DeserializeObject<OfferSettingsDisplay>(json);

            var offerRedeemed = new OfferRedeemed(
                offerSettings.OfferCode,
                offerSettings.OfferProviderKey,
                lineItem.ContainerKey,
                offerSettings.Key);

            offerRedeemed.CustomerKey = _customerKey;

            offerRedeemed.ExtendedData.SetValue(Core.Constants.ExtendedDataKeys.CouponReward, json);

            _redemptions.Add(offerRedeemed);
        }
    }
}