using Umbraco.Core;

namespace Merchello.Core.Chains
{
    /// <summary>
    /// Defines a pipeline task
    /// </summary>
    public interface IAttemptChainTask<T>
    {
        /// <summary>
        /// Defines an attempt chain task
        /// </summary>
        Attempt<T> PerformTask(T arg);
    }
}