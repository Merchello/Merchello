namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Merchello.Core.Models;

    /// <summary>
    /// Represents an offer component definition.
    /// </summary>
    public class OfferComponentDefinition : IHasExtendedData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentDefinition"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public OfferComponentDefinition(OfferComponentConfiguration configuration)
        {
            Ensure.ParameterNotNull(configuration, "configuration");
            this.OfferSettingsKey = configuration.OfferSettingsKey;
            this.OfferCode = configuration.OfferCode;
            this.ComponentKey = configuration.ComponentKey;
            this.TypeFullName = configuration.TypeFullName;
            this.ExtendedData = configuration.Values.AsExtendedDataCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentDefinition"/> class.
        /// </summary>
        internal OfferComponentDefinition()
        {
            this.ExtendedData = new ExtendedDataCollection();
        }

        /// <summary>
        /// Gets or sets the offer settings key.
        /// </summary>
        public Guid OfferSettingsKey { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        public string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the component key.
        /// </summary>
        public Guid ComponentKey { get; set; }

        /// <summary>
        /// Gets the component type name.
        /// </summary>
        public string TypeFullName { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="ExtendedDataCollection"/>.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; }
    }
}