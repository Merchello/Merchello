namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Merchello.Core.Models;
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

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="T">
        /// The type of offer award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt{IOfferAwardResult}"/>.
        /// </returns>
        public Attempt<IOfferAwardResult<T>> TryToAward<T>(ICustomerBase customer) where T : class
        {
            return TryToAward<T>(null, customer);
        }

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="constraintBy">
        /// An object passed to the offer constraints.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="T">
        /// The type of offer award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt{IOfferAwardResult}"/>.
        /// </returns>
        public Attempt<IOfferAwardResult<T>> TryToAward<T>(object constraintBy, ICustomerBase customer) where T : class
        {
            var attempt = TryToAward(constraintBy, customer);

            if (!attempt.Success)
            {
                var failed = Attempt<IOfferAwardResult<T>>.Fail(attempt.Exception);
                if (attempt.Result != null)
                {
                    failed.Result.Messages = attempt.Result.Messages;
                }

                return failed;
            }
            
            var success = Attempt<IOfferAwardResult<T>>.Succeed(new OfferRewardResult<T>());
            success.Result.Award = attempt.Result as T;
            success.Result.Messages = attempt.Result.Messages;
            return success;
        }

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<IOfferAwardResult<object>> TryToAward(ICustomerBase customer)
        {
            return TryToAward(null, customer);
        }

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="constrainBy">
        /// The constrain by.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public abstract Attempt<IOfferAwardResult<object>> TryToAward(object constrainBy, ICustomerBase customer);
        

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