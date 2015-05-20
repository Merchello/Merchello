namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Merchello.Core.Models;

    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// The constraint settings.
    /// </summary>
    public class OfferComponentDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentDefinition"/> class.
        /// </summary>
        public OfferComponentDefinition()
        {
            this.ExtendedData = new ExtendedDataCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentDefinition"/> class.
        /// </summary>
        /// <param name="jsonConfiguration">
        /// The component config JSON.
        /// </param>
        public OfferComponentDefinition(string jsonConfiguration)
            : this(JsonConvert.DeserializeObject<OfferComponentConfiguration>(jsonConfiguration))
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentDefinition"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        internal OfferComponentDefinition(OfferComponentConfiguration configuration)
        {
            Mandate.ParameterNotNull(configuration, "configuration");
            this.ComponentKey = configuration.ComponentKey;
            this.ExtendedData = configuration.Values.AsExtendedDataCollection();
        }

        /// <summary>
        /// Gets or sets the component key.
        /// </summary>
        public Guid ComponentKey { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ExtendedDataCollection"/>.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; }
    }
}