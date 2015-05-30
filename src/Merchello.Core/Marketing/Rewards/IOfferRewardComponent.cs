namespace Merchello.Core.Marketing.Rewards
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines a reward.
    /// </summary>
    /// <typeparam name="T">
    /// The type of reward to be constructed and returned
    /// </typeparam>
    public interface IOfferRewardComponent<T>
    {
        /// <summary>
        /// Awards the reward.
        /// </summary>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <returns>
        /// A value indicating whether or not the awarding process was successful.
        /// </returns>
        Attempt<T> Award(ICustomerBase customer, ILineItemContainer container);
    }
}