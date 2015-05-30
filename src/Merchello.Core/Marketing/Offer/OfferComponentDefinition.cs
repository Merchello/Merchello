namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The constraint settings.
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
            Mandate.ParameterNotNull(configuration, "configuration");
            this.ComponentKey = configuration.ComponentKey;
            this.TypeName = configuration.TypeName;
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
        /// Gets or sets the component key.
        /// </summary>
        public Guid ComponentKey { get; set; }

        /// <summary>
        /// Gets the component type name.
        /// </summary>
        public string TypeName { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="ExtendedDataCollection"/>.
        /// </summary>
        public ExtendedDataCollection ExtendedData { get; set; }
    }
}