namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;

    /// <summary>
    /// A base for Offer classes
    /// </summary>
    public abstract class OfferBase : IOffer 
    {       
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferBase"/> class.
        /// </summary>
        /// <param name="settings">
        /// The <see cref="IOfferSettings"/>.
        /// </param>
        protected OfferBase(IOfferSettings settings)
        {
            Mandate.ParameterNotNull(settings, "settings");

            this.Settings = settings;
        }

        #region Properties

        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <remarks>
        /// This is actually a reference to the OfferSettings key
        /// </remarks>
        public Guid Key 
        {
            get
            {
                return Settings.Key;
            }        
        }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        public Guid OfferProviderKey
        {
            get
            {
                return Settings.OfferProviderKey;
            }

            set
            {
                Settings.OfferProviderKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return Settings.Name;
            }

            set
            {
                Settings.Name = value;
            }
        }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        public virtual string OfferCode
        {
            get
            {
                return Settings.OfferCode;
            }

            set
            {
                Settings.OfferCode = value;
            }
        }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        public virtual DateTime OfferStartsDate
        {
            get
            {
                return Settings.OfferStartsDate;
            }

            set
            {
                Settings.OfferStartsDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        public virtual DateTime OfferEndsDate
        {
            get
            {
                return Settings.OfferEndsDate;
            }

            set
            {
                Settings.OfferEndsDate = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the offer is active.
        /// </summary>
        public virtual bool Active
        {
            get
            {
                return Settings.Active;
            }

            set
            {
                Settings.Active = value;
            }
        }

        /// <summary>
        /// Gets the settings.
        /// </summary>
        internal IOfferSettings Settings { get; private set; }

        #endregion

        /// <summary>
        /// Adds a <see cref="OfferComponentBase"/> to the offer.
        /// </summary>
        /// <param name="component">
        /// The component.
        /// </param>
        internal virtual void AddComponent(OfferComponentBase component)
        {
            AddComponent(component.OfferComponentDefinition);
        }

        /// <summary>
        /// Adds a <see cref="OfferComponentDefinition"/> to the offer.
        /// </summary>
        /// <param name="definition">
        /// The component definition.
        /// </param>
        internal void AddComponent(OfferComponentDefinition definition)
        {
            Settings.ComponentDefinitions.Add(definition);
        }

    }
}