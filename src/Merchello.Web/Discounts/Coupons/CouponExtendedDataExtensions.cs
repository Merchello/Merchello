namespace Merchello.Web.Discounts.Coupons
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Web.Models.ContentEditing;

    using Newtonsoft.Json;

    using Umbraco.Core.Logging;

    /// <summary>
    /// ExtendedDataCollection extensions for coupon discounts.
    /// </summary>
    public static class CouponExtendedDataExtensions 
    {
        /// <summary>
        /// Determines whether or not the <see cref="ILineItemContainer"/> contains ANY serialized coupons.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the <see cref="ILineItemContainer"/> contains ANY serialized coupons..
        /// </returns>
        public static bool ContainsAnyCoupons(this ILineItemContainer container)
        {
            return container.Items.Any(x => x.ContainsCoupon());
        }

        /// <summary>
        /// Determines whether or not the <see cref="ILineItem"/> contains a serialized coupon.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the <see cref="ILineItem"/> contains a serialized coupon.
        /// </returns>
        public static bool ContainsCoupon(this ILineItem lineItem)
        {
            return lineItem.ExtendedData.ContainsCoupon();
        }

        /// <summary>
        /// Determines whether or not the <see cref="ExtendedDataCollection"/> contains a serialized coupon.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the <see cref="ExtendedDataCollection"/> contains a serialized coupon.
        /// </returns>
        public static bool ContainsCoupon(this ExtendedDataCollection extendedData)
        {
            return extendedData.ContainsKey(Core.Constants.ExtendedDataKeys.CouponReward);
        }

        /// <summary>
        /// Determines whether or not the <see cref="ExtendedDataCollection"/> has a coupon constraint
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="constraintId">
        /// The constraint id.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the <see cref="ExtendedDataCollection"/> contains a specific coupon constraint.
        /// </returns>
        public static bool HasCouponConstraint(this ExtendedDataCollection extendedData, Guid constraintId)
        {
            if (!extendedData.ContainsKey(Core.Constants.ExtendedDataKeys.CouponReward))
                return false;

            var offerSettings = extendedData.GetOfferSettingsDisplay();
            if (offerSettings == null)
                return false;

            return offerSettings.ComponentDefinitions.Any(x => x.ComponentKey == constraintId);
        }

        /// <summary>
        /// Gets a collection of <see cref="OfferSettingsDisplay"/> from an <see cref="ILineItemContainer"/>.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{OfferSettingsDisplay}"/>.
        /// </returns>
        internal static IEnumerable<OfferSettingsDisplay> GetOfferSettingsDisplays(this ILineItemContainer container)
        {
            if (!container.ContainsAnyCoupons()) return Enumerable.Empty<OfferSettingsDisplay>();
            return container.Items.Select(GetOfferSettingsDisplay);
        } 

        /// <summary>
        /// Gets a <see cref="OfferSettingsDisplay"/> from an <see cref="ILineItem"/>.
        /// </summary>
        /// <param name="lineItem">
        /// The line item.
        /// </param>
        /// <returns>
        /// The <see cref="OfferSettingsDisplay"/>.
        /// </returns>
        internal static OfferSettingsDisplay GetOfferSettingsDisplay(this ILineItem lineItem)
        {
            return lineItem.ExtendedData.GetOfferSettingsDisplay();
        }

        /// <summary>
        /// Gets a <see cref="OfferSettingsDisplay"/> from an <see cref="ExtendedDataCollection"/>.
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <returns>
        /// The <see cref="OfferSettingsDisplay"/>.
        /// </returns>
        internal static OfferSettingsDisplay GetOfferSettingsDisplay(this ExtendedDataCollection extendedData)
        {
            if (!extendedData.ContainsCoupon()) return null;

            try
            {
                return 
                    JsonConvert.DeserializeObject<OfferSettingsDisplay>(
                        extendedData.GetValue(Core.Constants.ExtendedDataKeys.CouponReward));
                
            }
            catch (Exception ex)
            {
                LogHelper.Error(typeof(CouponExtendedDataExtensions), "Failed to deserialize coupon from ExtendedDataCollection", ex);
                throw;
            }
        }

        /// <summary>
        /// Serialized and stores the coupon into the <see cref="ExtendedDataCollection"/>
        /// </summary>
        /// <param name="extendedData">
        /// The extended data.
        /// </param>
        /// <param name="coupon">
        /// The coupon.
        /// </param>
        internal static void SetCouponValue(this ExtendedDataCollection extendedData, ICoupon coupon)
        {
            extendedData.SetValue(
                Core.Constants.ExtendedDataKeys.CouponReward,
                JsonConvert.SerializeObject(((Coupon)coupon).Settings.ToOfferSettingsDisplay()));
        }
    }
}