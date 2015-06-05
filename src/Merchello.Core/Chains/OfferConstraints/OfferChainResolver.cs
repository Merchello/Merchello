namespace Merchello.Core.Chains.OfferConstraints
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Merchello.Core.Marketing.Constraints;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.ObjectResolution;

    /// <summary>
    /// The offer constraint chain resolver.
    /// </summary>
    internal class OfferChainResolver : ResolverBase<OfferChainResolver>, IOfferChainResolver
    {
        /// <summary>
        /// The instance types.
        /// </summary>
        private readonly List<Type> _instanceTypes = new List<Type>();

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferChainResolver"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public OfferChainResolver(IEnumerable<Type> values)
        {
            _instanceTypes = values.ToList();
        }


        /// <summary>
        /// The get chain constrain by type.
        /// </summary>
        /// <param name="constraints">
        /// The constrain by type
        /// </param>
        /// <returns>
        /// The <see cref="IOfferAttemptChain"/>.
        /// </returns>
        public IOfferAttemptChain BuildChain(IEnumerable<OfferConstraintComponentBase> constraints, Type rewardType)
        {
            var constraintsArray = constraints as OfferConstraintComponentBase[] ?? constraints.ToArray();
            if (!constraintsArray.Any()) return null;

            var constraintType = constraintsArray.First().TypeGrouping;

            var chainType =
                _instanceTypes.FirstOrDefault(
                    x => x.GetCustomAttribute<OfferConstraintChainForAttribute>(false).ConstraintType == constraintType && 
                        x.GetCustomAttribute<OfferConstraintChainForAttribute>(false).RewardType == rewardType);

            if (chainType == null) return null;


            var ctrArgs = new object[] { constraints };
            var attempt = ActivatorHelper.CreateInstance<IOfferAttemptChain>(chainType, ctrArgs);
            if (!attempt.Success)
            {
                LogHelper.Error<OfferChainResolver>("Failed to create instance of " + chainType.Name, attempt.Exception);
                throw attempt.Exception;
            }

            return attempt.Result;
        }
    }
}