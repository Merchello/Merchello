namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Marketing.Offer;

    /// <summary>
    /// The offer provider display.
    /// </summary>
    public class OfferProviderDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name of the type managed by this provider.
        /// </summary>
        public string ManagesTypeName { get; set; }

        /// <summary>
        /// Gets or sets the back office tree configurations.
        /// </summary>
        public BackOfficeTreeDisplay BackOfficeTree { get; set; }
    }

    /// <summary>
    /// Utility mapping extensions for <see cref="OfferProviderDisplay"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class OfferProviderDisplayExtensions
    {
        /// <summary>
        /// Maps a <see cref="IOfferProvider"/> to a <see cref="OfferProviderDisplay"/>.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <returns>
        /// The <see cref="OfferProviderDisplay"/>.
        /// </returns>
        public static OfferProviderDisplay ToOfferProviderDisplay(this IOfferProvider provider)
        {
            return AutoMapper.Mapper.Map<IOfferProvider, OfferProviderDisplay>(provider);
        }
    }
}