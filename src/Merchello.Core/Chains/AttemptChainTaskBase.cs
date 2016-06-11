namespace Merchello.Core.Chains
{
    using Umbraco.Core;

    /// <summary>
    /// The attempt chain task base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of value
    /// </typeparam>
    public abstract class AttemptChainTaskBase<T> : IAttemptChainTask<T>
    {
        /// <summary>
        /// Defines an attempt chain task
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public abstract Attempt<T> PerformTask(T value);
    }
}