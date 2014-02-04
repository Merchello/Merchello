using System.Collections.Generic;
using Umbraco.Core;

namespace Merchello.Core.Pipelines
{
    /// <summary>
    /// Marker interface for Pipelines.  This is a specialized chain of responsibility.
    /// </summary>
    internal interface IPipelineBase<in T> 
        where T : IPipelineTask
    {
        void RegisterTask(T task);

        Attempt<IPipelineEndOfChain> Execute();

    }
}