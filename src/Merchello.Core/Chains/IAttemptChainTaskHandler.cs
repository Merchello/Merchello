using Umbraco.Core;

namespace Merchello.Core.Chains
{
    public interface IAttemptChainTaskHandler<T>
    {
        Attempt<T> Execute(T arg);

        void RegisterNext(IAttemptChainTaskHandler<T> next);
    }
    
}