namespace Merchello.Core.Marketing.Rewards
{
    /// <summary>
    /// Defines a reward.
    /// </summary>
    public interface IReward
    {
        /// <summary>
        /// Awards the reward.
        /// </summary>
        /// <returns>
        /// A value indicating whether or not the awarding process was successful.
        /// </returns>
        bool Award();
    }
}