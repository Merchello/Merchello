namespace Merchello.Web.Models.ContentEditing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Core;

    using Merchello.Core.Models;

    /// <summary>
    /// The gateway provider display.
    /// </summary>
    public class GatewayProviderDisplay : DialogEditorDisplayBase
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the provider type field key.
        /// </summary>
        public Guid ProviderTfKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the extended data.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether encrypt extended data.
        /// </summary>
        public bool EncryptExtendedData { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether activated.
        /// </summary>
        public bool Activated { get; set; }
    }

    #region Mapping Extensions

    /// <summary>
    /// Mapping extensions for <see cref="GatewayProviderDisplay"/> classes.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class GatewayProviderDisplayExtensions
    {
        /// <summary>
        /// Maps <see cref="IGatewayProviderSettings"/> to <see cref="GatewayProviderDisplay"/>.
        /// </summary>
        /// <param name="gatewayProviderSettings">
        /// The gateway provider settings.
        /// </param>
        /// <returns>
        /// The <see cref="GatewayProviderDisplay"/>.
        /// </returns>
        internal static GatewayProviderDisplay ToGatewayProviderDisplay(this IGatewayProviderSettings gatewayProviderSettings)
        {
            return AutoMapper.Mapper.Map<GatewayProviderDisplay>(gatewayProviderSettings);
        }

        /// <summary>
        /// Maps <see cref="GatewayProviderDisplay"/> to <see cref="IGatewayProviderSettings"/>.
        /// </summary>
        /// <param name="gatewayProvider">
        /// The gateway provider.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <returns>
        /// The <see cref="IGatewayProviderSettings"/>.
        /// </returns>
        internal static IGatewayProviderSettings ToGatewayProvider(this GatewayProviderDisplay gatewayProvider, IGatewayProviderSettings destination)
        {
            if (gatewayProvider.Key != Guid.Empty) destination.Key = gatewayProvider.Key;
            // type key and typeFullName should be handled by the resolver 
            destination.Name = gatewayProvider.Name;
            destination.Description = gatewayProvider.Description;
            destination.EncryptExtendedData = gatewayProvider.EncryptExtendedData;

            ((GatewayProviderSettings)destination).ExtendedData = gatewayProvider.ExtendedData.AsExtendedDataCollection();

            return destination;
        }
    }

    #endregion
}
