namespace Merchello.Core.Marketing.Rewards
{
    using Merchello.Core.Models;

    /// <summary>
    /// Defines a reward.
    /// </summary>
    public interface IOfferRewardComponent
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
        bool Award(ICustomerBase customer, ILineItemContainer container);
    }
}