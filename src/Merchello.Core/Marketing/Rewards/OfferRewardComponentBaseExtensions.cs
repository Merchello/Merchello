namespace Merchello.Core.Marketing.Rewards
{
    using System;

    using Merchello.Core.Marketing.Offer;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The offer reward component base extensions.
    /// </summary>
    internal static class OfferRewardComponentBaseExtensions
    {
        /// <summary>
        /// Gets the applyToEachMatching from the reward extended data collection.
        /// </summary>
        /// <param name="reward">
        /// The reward.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetApplyAwardToEachMatching(this OfferRewardComponentBase reward)
        {
            if (!reward.OfferComponentDefinition.ExtendedData.ContainsKey("applyToEachMatching")) return true;
            
            bool converted;
            return bool.TryParse(reward.OfferComponentDefinition.ExtendedData.GetValue("applyToEachMatching"), out converted) && converted;
        }

        /// <summary>
        /// The get reward line item name.
        /// </summary>
        /// <param name="reward">
        /// The reward.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetRewardLineItemName(this OfferRewardComponentBase reward)
        {
            var att = reward.GetOfferComponentAttribute();

            if (!reward.OfferComponentDefinition.ExtendedData.ContainsKey("lineItemName"))
            {                
                return att.Name;
            }

            var name = reward.OfferComponentDefinition.ExtendedData.GetValue("lineItemName");

            return string.IsNullOrEmpty(name) ? att.Name : name;
        }

        /// <summary>
        /// The get reward line item name.
        /// </summary>
        /// <param name="reward">
        /// The reward.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetRewardOfferCode(this OfferRewardComponentBase reward)
        {
            var defaultCode = reward.OfferComponentDefinition.ComponentKey.ToString().Substring(0, 8);
            if (!reward.OfferComponentDefinition.ExtendedData.ContainsKey("offerCode"))
            {
                LogHelper.Debug(typeof(OfferRewardComponentBaseExtensions), "offerCode was not set in extended data");
                return defaultCode;
            }

            var offerCode = reward.OfferComponentDefinition.ExtendedData.GetValue("offerCode");

            return string.IsNullOrEmpty(offerCode) ? defaultCode : offerCode;
        }
    }
}