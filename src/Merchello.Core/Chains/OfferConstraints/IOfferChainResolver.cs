namespace Merchello.Core.Chains.OfferConstraints
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Constraints;

    /// <summary>
    /// Defines a OfferChainResolver.
    /// </summary>
    internal interface IOfferChainResolver
    {
        /// <summary>
        /// The get chain constrain by type.
        /// </summary>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        /// <param name="rewardType">
        /// The reward Type.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferAttemptChain"/>.
        /// </returns>
        IOfferAttemptChain BuildChain(IEnumerable<OfferConstraintComponentBase> constraints, Type rewardType);
    }
}