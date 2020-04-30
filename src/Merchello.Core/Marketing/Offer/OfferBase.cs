namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Chains.OfferConstraints;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;

    using umbraco.cms.businesslogic.datatype;

    using Umbraco.Core;

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
        private IOfferProcessorFactory _offerProcessorFactory;

        /// <summary>
        /// The offer processor.
        /// </summary>
        private Lazy<IOfferProcessor> _offerProcessor;

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
            Ensure.ParameterNotNull(settings, "settings");
            
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
        internal IEnumerable<OfferConstraintComponentBase> Constraints
        {
            get
            {
                return ResolvedComponents.Where(x => x.ComponentType == OfferComponentType.Constraint).Select(x => (OfferConstraintComponentBase)x);
            }
        }

        /// <summary>
        /// Gets the offer reward.
        /// </summary>
        internal OfferRewardComponentBase Reward
        {
            get
            {
                return ResolvedComponents.FirstOrDefault(x => x.ComponentType == OfferComponentType.Reward) as OfferRewardComponentBase;
            }
        }

        /// <summary>
        /// Gets the offer processor.
        /// </summary>
        internal IOfferProcessor OfferProcessor
        {
            get
            {
                return _offerProcessor.Value; 
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

        #region line item offer      

        /// <summary>
        /// Attempts to apply the constraints against the offer.
        /// </summary>
        /// <param name="validatedAgainst">
        /// The validated against.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<IOfferResult<TConstraint, TAward>> TryApplyConstraints<TConstraint, TAward>(object validatedAgainst, ICustomerBase customer) 
            where TConstraint : class 
            where TAward : class
        {
            var result = new OfferResult<object, object>
            {
                Customer = customer,
                Messages = new List<string>()
            };

            // ensure the offer is valid
            var ensureOffer = this.EnsureValidOffer(result);
            if (!ensureOffer.Success) return ensureOffer.As<TConstraint, TAward>();
            result = ensureOffer.Result as OfferResult<object, object>;
            if (result == null) throw new NullReferenceException("EnsureValidOffer returned a null Result");

            // attempt to validate against any constraints that have been configured
            var constraintAttempt = TryApplyConstraints(validatedAgainst, customer);
            result = PopulateConstraintOfferResult(result, constraintAttempt) as OfferResult<object, object>;
            if (result == null) throw new NullReferenceException("EnsureValidOffer returned a null Result");

            if (!constraintAttempt.Success)
            {
                var exception = new Exception("Offer constraint validation failed.");
                return Attempt<IOfferResult<object, object>>.Fail(result, exception).As<TConstraint, TAward>();
            }

            result.ValidatedAgainst = validatedAgainst as TConstraint;
            result.Messages.Add("PASSED - Constraint validation");
            return Attempt<IOfferResult<object, object>>.Succeed(result).As<TConstraint, TAward>();
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
        /// <param name="applyConstraints">
        /// Optional parameter indicating whether or not to apply constraints before attempting to award the reward.
        /// Defaults to true.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt{IOfferResult}"/>.
        /// </returns>
        public Attempt<IOfferResult<TConstraint, TAward>> TryToAward<TConstraint, TAward>(object validatedAgainst, ICustomerBase customer, bool applyConstraints = true) 
            where TAward : class
            where TConstraint : class
        {
            return TryToAward(validatedAgainst, customer, applyConstraints).As<TConstraint, TAward>();           
        }


        /// <summary>
        /// Tries to apply the constraints
        /// </summary>
        /// <param name="validatedAgainst">
        /// The validated against.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        internal Attempt<object> TryApplyConstraints(object validatedAgainst, ICustomerBase customer)
        {
            // apply the constraints
            return OfferProcessor.TryApplyConstraints(validatedAgainst, customer);
        }

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="applyConstraints">
        /// Optional parameter indicating whether or not to apply constraints before attempting to award the reward.
        /// Defaults to true.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt{IOfferResult}"/>.
        /// </returns>
        internal Attempt<IOfferResult<TConstraint, TAward>> TryToAward<TConstraint, TAward>(ICustomerBase customer, bool applyConstraints = true)
            where TAward : class
            where TConstraint : class
        {
            return TryToAward<TConstraint, TAward>(null, customer, applyConstraints);
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
        /// <param name="applyConstraints">
        /// Optional parameter indicating whether or not to apply constraints before attempting to award the reward.
        /// Defaults to true.
        /// </param>
        /// <returns>
        /// 
        /// The <see cref="Attempt"/>.
        /// </returns>
        internal Attempt<IOfferResult<object, object>> TryToAward(object validatedAgainst, ICustomerBase customer, bool applyConstraints = true)
        {
            var result = new OfferResult<object, object>
                             {   
                                 Customer = customer,
                                 Messages = new List<string>()
                             };

            // ensure the offer is valid
            var ensureOffer = this.EnsureValidOffer(result);
            if (!ensureOffer.Success) return ensureOffer;
            result = ensureOffer.Result as OfferResult<object, object>;
            if (result == null) throw new NullReferenceException("EnsureValidOffer returned a null Result");

            Attempt<object> awardAttempt;

            if (applyConstraints)
            {
                // apply the constraints
                var constraintAttempt = this.TryApplyConstraints(validatedAgainst, customer);
                result = PopulateConstraintOfferResult(result, constraintAttempt) as OfferResult<object, object>;
                if (result == null) throw new NullReferenceException("PopulateConstraintOfferResult returned null");

                if (!constraintAttempt.Success)
                {
                    return Attempt<IOfferResult<object, object>>.Fail(result, constraintAttempt.Exception);
                }

                awardAttempt = OfferProcessor.TryAward(constraintAttempt.Result, customer);
                result.ValidatedAgainst = constraintAttempt.Result;
            }
            else
            {
                awardAttempt = OfferProcessor.TryAward(validatedAgainst, customer);
                result.ValidatedAgainst = validatedAgainst;
            }

            // get the reward            
            result.Award = awardAttempt.Result;
            if (!awardAttempt.Success)
            {
                return Attempt<IOfferResult<object, object>>.Fail(result, awardAttempt.Exception);
            }

            result.Messages.Add("AWARD - Success");
            return Attempt<IOfferResult<object, object>>.Succeed(result);
        }

        #endregion

        /// <summary>
        /// The ensure offer is valid.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        internal Attempt<IOfferResult<TConstraint, TAward>> EnsureOfferIsValid<TConstraint, TAward>(ICustomerBase customer)
            where TConstraint :
            class
            where TAward : class
        {
            var result = new OfferResult<object, object>
            {
                Customer = customer,
                Messages = new List<string>()
            };

            return this.EnsureValidOffer(result).As<TConstraint, TAward>();
        }

        /// <summary>
        /// The populate constraint offer result.
        /// </summary>
        /// <param name="seed">
        /// The seed.
        /// </param>
        /// <param name="constraintAttempt">
        /// The constraint attempt.
        /// </param>
        /// <returns>
        /// The offer result.
        /// </returns>
        private static IOfferResult<object, object> PopulateConstraintOfferResult(IOfferResult<object, object> seed, Attempt<object> constraintAttempt)
        {
            if (!constraintAttempt.Success)
            {
                seed.Award = null;
                seed.ValidatedAgainst = constraintAttempt.Result;
                seed.Messages.AddRange(
                    new[] { "Did not pass constraint validation.", constraintAttempt.Exception.Message });
            }
            else
            {
                seed.Messages.Add("PASSED - Constraint validation");
            }

            return seed;
        }

        /// <summary>
        /// Ensures the offer is valid.
        /// </summary>
        /// <param name="seed">
        /// The seed.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        private Attempt<IOfferResult<object, object>> EnsureValidOffer(IOfferResult<object, object> seed)
        {
            // verify a reward has been configured
            if (Reward == null)
            {
                seed.Messages.Add("This offer does not have a reward configured");
                return Attempt<IOfferResult<object, object>>.Fail(seed, new OfferRedemptionException("Offer does not have a configured award"));
            }

            // ensure the offer is active
            if (!Settings.Active)
            {
                seed.Messages.Add("Offer is not active");
                return Attempt<IOfferResult<object, object>>.Fail(seed, new OfferRedemptionException("Offer is not active"));
            }

            // ensure the offer has not expired
            if (Settings.Expired)
            {
                seed.Messages.Add("Offer has expired");
                return Attempt<IOfferResult<object, object>>.Fail(seed, new OfferRedemptionException("Offer has expired"));
            }

            // verify an offer processor is available for the offer
            if (OfferProcessor == null)
            {
                seed.Messages.Add("An offer processor could not be resolved for this offer.  Custom offers must have custom offer processors defined.");
                return Attempt<IOfferResult<object, object>>.Fail(seed, new OfferRedemptionException("Offer processor was not resolved"));
            }

            // success
            return Attempt<IOfferResult<object, object>>.Succeed(seed);
        } 


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

            if (!OfferProcessorFactory.HasCurrent) throw new Exception("OfferProcessorFactory has not been instantiated");
            this._offerProcessorFactory = OfferProcessorFactory.Current;

            // wire up the offer processor
            _offerProcessor = new Lazy<IOfferProcessor>(() => _offerProcessorFactory.Build(this));
        }

    }
}