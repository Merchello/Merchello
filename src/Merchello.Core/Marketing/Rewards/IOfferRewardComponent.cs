namespace Merchello.Core.Marketing.Rewards
{
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines a reward.
    /// </summary>
    /// <typeparam name="TConstraint">
    /// The type to be passed to the constraints collection to validate if the reward should be awarded
    /// </typeparam>
    /// <typeparam name="TReward">
    /// The type of award to be returned
    /// </typeparam>
    public interface IOfferRewardComponent<in TConstraint, TReward>
    {
        /// <summary>
        /// Awards the reward.
        /// </summary>
        /// <param name="validate">
        /// The object to pass to the constraints collection
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the awarding process was successful.
        /// </returns>
        Attempt<TReward> TryAward(TConstraint validate, ICustomerBase customer);
    }
}