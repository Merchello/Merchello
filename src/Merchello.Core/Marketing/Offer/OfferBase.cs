namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using Merchello.Core.Chains.OfferConstraints;
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
        /// The offer chain resolver.
        /// </summary>
        private IOfferChainResolver _offerChainResolver;

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

        #endregion

        #region lineitemoffer

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt{IOfferResult}"/>.
        /// </returns>
        public Attempt<IOfferResult<TAward, TConstraint>> TryToAward<TAward, TConstraint>(ICustomerBase customer) 
            where TAward : class
            where TConstraint : class
        {
            return TryToAward<TAward, TConstraint>(null, customer);
        }

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="validatedAgainst">
        /// An object passed to the offer constraints.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt{IOfferResult}"/>.
        /// </returns>
        public Attempt<IOfferResult<TAward, TConstraint>> TryToAward<TAward, TConstraint>(object validatedAgainst, ICustomerBase customer) 
            where TAward : class
            where TConstraint : class
        {
            return TryToAward(validatedAgainst, customer).As<TAward, TConstraint>();           
        }


        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="validatedAgainst">
        /// The constrain by.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// 
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<IOfferResult<object, object>> TryToAward(object validatedAgainst, ICustomerBase customer)
        {
            var result = new OfferResult<object, object>
                             {   
                                 Customer = customer,
                                 Messages = new List<string>()
                             };

            // assert there is a reward
            if (Reward == null)
            {
                var nullReference = new NullReferenceException("Reward property was null");
                result.Award = null;
                result.Messages.Add("A reward has not been set for this offer.");
                return Attempt<IOfferResult<object, object>>.Fail(result, nullReference);
            }


            var offerChain = this._offerChainResolver.BuildChain(Constraints, Reward.RewardType);
            
            // apply the constraints
            var constraintAttempt = offerChain.TryApplyConstraints(validatedAgainst, customer);
            if (!constraintAttempt.Success)
            {
                var exception = new Exception("Offer constraint validation failed.");
                result.Award = null;
                result.ValidatedAgainst = constraintAttempt.Result;
                result.Messages.AddRange(
                            new[] 
                            {
                                "Did not pass constraint validation.",
                                constraintAttempt.Exception.Message 
                            });
                return Attempt<IOfferResult<object, object>>.Fail(result, exception);
            }

            // get the reward
            var awardAttempt = offerChain.TryAward(constraintAttempt.Result, customer);
            result.Award = awardAttempt.Result;
            result.ValidatedAgainst = constraintAttempt.Result;
            if (awardAttempt.Success)
            {                
                result.Messages.Add("Success");
                return Attempt<IOfferResult<object, object>>.Succeed(result);
            }

            return Attempt<IOfferResult<object, object>>.Fail(result, awardAttempt.Exception);
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

            if (!OfferChainResolver.HasCurrent) throw new Exception("OfferChainResolver has not been instantiated");
            this._offerChainResolver = OfferChainResolver.Current;

        }

    }
}