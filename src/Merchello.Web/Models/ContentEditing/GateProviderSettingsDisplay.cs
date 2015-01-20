namespace Merchello.Web.Models.ContentEditing
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Models;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The gate provider settings display.
    /// </summary>
    public class GateProviderSettingsDisplay
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
        /// Gets or sets a value indicating whether encrypt extended data.
        /// </summary>
        public bool EncryptExtendedData { get; set; }

        /// <summary>
        /// Gets a value indicating whether activated.
        /// </summary>
        public bool Activated { get; private set; }

        /// <summary>
        /// Gets the gateway provider type.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public GatewayProviderType GatewayProviderType { get; private set; }

        /// <summary>
        /// Gets the extended data.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; private set; }
    }
}