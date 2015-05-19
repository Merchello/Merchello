namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Merchello.Core.Models;

    using Newtonsoft.Json;

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
            ExtendedData = new ExtendedDataCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentDefinition"/> class.
        /// </summary>
        /// <param name="jsonConfiguration">
        /// The component config JSON.
        /// </param>
        public OfferComponentDefinition(string jsonConfiguration)
        {
            var config = JsonConvert.DeserializeObject<OfferComponentConfiguration>(jsonConfiguration);
            ComponentKey = config.ComponentKey;
            ExtendedData = config.Values.AsExtendedDataCollection();
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