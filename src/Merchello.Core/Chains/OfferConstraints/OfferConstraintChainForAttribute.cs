namespace Merchello.Core.Chains.OfferConstraints
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// The offer constraint chain for attribute.
    /// </summary>
    public class OfferConstraintChainForAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferConstraintChainForAttribute"/> class.
        /// </summary>
        /// <param name="constraintType">
        /// The type of constraint.
        /// </param>
        /// <param name="rewardType">
        /// The reward Type.
        /// </param>
        public OfferConstraintChainForAttribute(Type constraintType, Type rewardType)
        {
            Ensure.ParameterNotNull(constraintType, "constraintType");
            Ensure.ParameterNotNull(rewardType, "rewardType");

            ConstraintType = constraintType;
            RewardType = rewardType;
        }

        /// <summary>
        /// Gets the chain type.
        /// </summary>
        public Type ConstraintType { get; private set; }

        /// <summary>
        /// Gets the reward type.
        /// </summary>
        public Type RewardType { get; private set; }
    }
}