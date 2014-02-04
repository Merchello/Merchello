using Umbraco.Core;

namespace Merchello.Core.Pipelines
{
    /// <summary>
    /// Defines a pipeline task
    /// </summary>
    internal interface IPipelineTask<T>
    {   
        Attempt<T> PerformTask(T arg);
    }
}