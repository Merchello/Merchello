namespace Merchello.Core.Marketing.Offer
{
    using Umbraco.Core;

    /// <summary>
    /// Utility extensions for OfferComponentBaseClasses
    /// </summary>
    internal static class OfferComponentBaseExtensions
    {
        /// <summary>
        /// Wrapper to retrieve the <see cref="OfferComponentAttribute"/>.
        /// </summary>
        /// <param name="component">
        /// The component.
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentAttribute"/>.
        /// </returns>
        public static OfferComponentAttribute GetOfferComponentAttribute(this OfferComponentBase component)
        {
            return component.GetType().GetCustomAttribute<OfferComponentAttribute>(false);
        }
    }
}