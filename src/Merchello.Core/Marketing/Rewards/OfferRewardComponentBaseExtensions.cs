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
    }
}