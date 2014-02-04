using Umbraco.Core;

namespace Merchello.Core.Pipelines
{
    internal interface IPipelineTaskHandler<T>
    {
        Attempt<T> Execute(T arg);

        void RegisterNext(IPipelineTaskHandler<T> next);
    }
    
}