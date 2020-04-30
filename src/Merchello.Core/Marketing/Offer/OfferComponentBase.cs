namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Merchello.Core.Models;

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
            Ensure.ParameterNotNull(definition, "definition");

            this._definition = definition;
        }

        /// <summary>
        /// Gets the offer settings key.
        /// </summary>
        public Guid OfferSettingsKey
        {
            get
            {
                return _definition.OfferSettingsKey;
            }
        }

        /// <summary>
        /// Gets the offer code.
        /// </summary>
        public string OfferCode
        {
            get
            {
                return _definition.OfferCode;
            }
        }


        /// <summary>
        /// Gets the component type.
        /// </summary>
        public abstract OfferComponentType ComponentType { get; }

        /// <summary>
        /// Gets a value indicating whether this component requires configuration.
        /// </summary>
        public virtual bool RequiresConfiguration
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public virtual string DisplayConfigurationFormat
        {
            get
            {
                var ed = OfferComponentDefinition.ExtendedData;
                if (ed.IsEmpty) return "''";

                var count = ed.Values.Count;
                var label = count > 1 ? "values" : "value";

                return string.Format("'{0} configuration {1}'", count, label);

            }
        }
               
        /// <summary>
        /// Gets the Type to which this component can be grouped with
        /// </summary>
        internal abstract Type TypeGrouping { get; }

        /// <summary>
        /// Gets the <see cref="OfferComponentDefinition"/>.
        /// </summary>
        internal OfferComponentDefinition OfferComponentDefinition
        {
            get
            {
                return this._definition;
            }
        }

        /// <summary>
        /// Gets a value from the extended data configuration.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetConfigurationValue(string key)
        {
            return OfferComponentDefinition.ExtendedData.GetValue(key);
        }
    }
}