namespace Merchello.Core.Chains
{
    using Umbraco.Core;

    /// <summary>
    /// Defines the AttemptChainTaskHandler
    /// </summary>
    /// <typeparam name="T">The type of value</typeparam>
    public interface IAttemptChainTaskHandler<T>
    {
        /// <summary>
        /// Attempt to execute the task.  If successful, executes the next task until.  This process is repeated until
        /// the end of chain Task is reached.
        /// </summary>
        /// <param name="arg">
        /// The type of value
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<T> Execute(T arg);

        /// <summary>
        /// Registers the next task in the chain.
        /// </summary>
        /// <param name="next">
        /// The next task.
        /// </param>
        void RegisterNext(IAttemptChainTaskHandler<T> next);
    }
    
}