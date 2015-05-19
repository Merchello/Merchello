namespace Merchello.Core.Marketing.Offer
{
    using Newtonsoft.Json;

    using Umbraco.Core;

    /// <summary>
    /// Base class for offer constraints.
    /// </summary>
    public abstract class OfferComponentBase
    {
        /// <summary>
        /// The <see cref="OfferComponentDefinition"/>.
        /// </summary>
        private readonly OfferComponentDefinition _definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferComponentBase"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        protected OfferComponentBase(OfferComponentDefinition definition)
        {
            Mandate.ParameterNotNull(definition, "definition");

            this._definition = definition;
        }

        /// <summary>
        /// Gets the <see cref="OfferComponentDefinition"/>.
        /// </summary>
        protected OfferComponentDefinition OfferComponentDefinition
        {
            get
            {
                return this._definition;
            }
        }

        /// <summary>
        /// The get offer component configuration.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal string GetOfferComponentConfigurationJson()
        {
            return JsonConvert.SerializeObject(this._definition.AsOfferComponentConfiguration(this.GetType().Name));
        }
    }
}