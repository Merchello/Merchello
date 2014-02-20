using Umbraco.Core;

namespace Merchello.Core.Chains
{
    public abstract class AttemptChainTaskBase<T> : IAttemptChainTask<T>
    {
        /// <summary>
        /// Defines an attempt chain task
        /// </summary>
        public abstract Attempt<T> PerformTask(T value);
    }
}