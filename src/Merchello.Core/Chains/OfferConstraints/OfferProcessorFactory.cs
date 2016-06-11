namespace Merchello.Core.Chains.OfferConstraints
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.ObjectResolution;

    /// <summary>
    /// The offer constraint chain resolver.
    /// </summary>
    internal class OfferProcessorFactory : ResolverBase<OfferProcessorFactory>, IOfferProcessorFactory
    {
        /// <summary>
        /// The instance types.
        /// </summary>
        private readonly List<Type> _instanceTypes = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferProcessorFactory"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public OfferProcessorFactory(IEnumerable<Type> values)
        {
            _instanceTypes = values.ToList();
        }

        /// <summary>
        /// Builds the <see cref="IOfferProcessor"/>
        /// </summary>
        /// <param name="offer">
        /// The offer.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferProcessor"/>.
        /// </returns>
        public IOfferProcessor Build(OfferBase offer)
        {
            var constraints = offer.Constraints;

            var constraintsType = offer.Reward.TypeGrouping;
            var rewardType = offer.Reward.RewardType;

            var chainType =
               _instanceTypes.FirstOrDefault(
                   x => x.GetCustomAttribute<OfferConstraintChainForAttribute>(false).ConstraintType == constraintsType &&
                       x.GetCustomAttribute<OfferConstraintChainForAttribute>(false).RewardType == rewardType);

            if (chainType == null) return null;


            var ctrArgs = new object[] { };
            var attempt = ActivatorHelper.CreateInstance<IOfferProcessor>(chainType, ctrArgs);
            if (!attempt.Success)
            {
                MultiLogHelper.Error<OfferProcessorFactory>("Failed to create instance of " + chainType.Name, attempt.Exception);
                return null;
            }

            // initialize the processor
            attempt.Result.Initialize(constraints, offer.Reward);

            return attempt.Result;
        }
    }
}