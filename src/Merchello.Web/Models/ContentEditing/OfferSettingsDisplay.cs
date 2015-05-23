namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The offer settings display.
    /// </summary>
    public class OfferSettingsDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        public Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        public DateTime OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        public DateTime OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether active.
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether date dependent.
        /// </summary>
        public bool DateDependent { get; set; }

        /// <summary>
        /// Gets or sets the component definitions.
        /// </summary>
        public IEnumerable<OfferComponentDefinitionDisplay> ComponentDefinitions { get; set; }
    }

    /// <summary>
    /// Utility extensions to map <see cref="IOfferSettings"/> to/from display classes.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class OfferSettingDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="IOfferSettings"/> to <see cref="OfferSettingsDisplay"/>.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        /// <returns>
        /// The <see cref="OfferSettingsDisplay"/>.
        /// </returns>
        public static OfferSettingsDisplay ToOfferSettingsDisplay(this IOfferSettings settings)
        {
            return AutoMapper.Mapper.Map<IOfferSettings, OfferSettingsDisplay>(settings);
        }
    }
}