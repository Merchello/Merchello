using Umbraco.Core;

namespace Merchello.Core.Chains
{
    /// <summary>
    /// Defines the AttemptChainTaskHandler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IAttemptChainTaskHandler<T>
    {
        /// <summary>
        /// Attempt to execute the task.  If successful, executes the next task until.  This process is repeated until
        /// the end of chain Task is reached.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        Attempt<T> Execute(T arg);

        /// <summary>
        /// Registers the next task in the chain.
        /// </summary>
        /// <param name="next"></param>
        void RegisterNext(IAttemptChainTaskHandler<T> next);
    }
    
}