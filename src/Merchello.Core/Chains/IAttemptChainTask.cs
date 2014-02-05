using Umbraco.Core;

namespace Merchello.Core.Chains
{
    /// <summary>
    /// Defines a pipeline task
    /// </summary>
    public interface IAttemptChainTask<T>
    {   
        Attempt<T> PerformTask(T arg);
    }
}