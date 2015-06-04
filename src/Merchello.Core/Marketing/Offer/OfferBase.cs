namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core;
    using Umbraco.Core.Media;

    /// <summary>
    /// A base for Offer classes
    /// </summary>
    public abstract class OfferBase : IOffer
    {
        /// <summary>
        /// The offer component resolver.
        /// </summary>
        private IOfferComponentResolver _componentResolver;

        /// <summary>
        /// The resolved offer components.
        /// </summary>
        private IEnumerable<OfferComponentBase> _components;

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

            // set the offer component resolver
            this.Initialize();
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
        /// Gets the collection constraints.
        /// </summary>
        protected IEnumerable<OfferConstraintComponentBase> Constraints
        {
            get
            {
                return _components.Where(x => x.ComponentType == OfferComponentType.Constraint).Select(x => (OfferConstraintComponentBase)x);
            }
        }

        /// <summary>
        /// Gets the offer reward.
        /// </summary>
        protected OfferRewardComponentBase Reward
        {
            get
            {
                return _components.FirstOrDefault(x => x.ComponentType == OfferComponentType.Reward) as OfferRewardComponentBase;
            }
        }

        /// <summary>
        /// Gets the resolved components.
        /// </summary>
        private IEnumerable<OfferComponentBase> ResolvedComponents
        {
            get
            {
                return _components ?? _componentResolver.GetOfferComponents(Settings.ComponentDefinitions);
            }            
        } 

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
            
            var success = Attempt<IOfferAwardResult<T>>.Succeed(new OfferAwardResult<T>());
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
        public Attempt<IOfferAwardResult<object>> TryToAward(object constrainBy, ICustomerBase customer)
        {
            string msg;

            // assert there is a reward
            if (Reward == null)
            {
                msg = "A reward has not been set for this offer.";
                var nullReference = new NullReferenceException(msg);
                var result = new OfferAwardResult<object>() { Award = null, Messages = new List<string> { msg } };
                return Attempt<IOfferAwardResult<object>>.Fail(result, nullReference);
            }

            // assert the constraining type (seed) is the same as the type passed in as a parameter
            if (Reward.TypeGrouping != constrainBy.GetType())
            {
                msg = "This offer cannot be constrained by the paramter 'constrainBy' passed.  Type should be "
                      + Reward.TypeGrouping.FullName;
                var typeException = new InvalidOperationException(msg);
                var result = new OfferAwardResult<object>() { Award = null, Messages = new List<string> { msg } };
                return Attempt<IOfferAwardResult<object>>.Fail(result, typeException);
            }

            foreach (var constraint in Constraints)
            {
                
            }
            throw new NotImplementedException();
        }
        

        #endregion

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <exception cref="Exception">
        /// Throws an exception if the offer component resolver has not been instantiated
        /// </exception>
        private void Initialize()
        {
            if (!OfferComponentResolver.HasCurrent) throw new Exception("OfferComponentResolver has not been instantiated.");
            _componentResolver = OfferComponentResolver.Current;
        }

    }
}