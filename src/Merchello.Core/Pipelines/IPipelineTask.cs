using Umbraco.Core;

namespace Merchello.Core.Pipelines
{
    /// <summary>
    /// Defines a pipeline task
    /// </summary>
    internal interface IPipelineTask
    {
        void Undo();

        void RegisterNext(IPipelineTask nextTask);

        Attempt<IPipelineTask> Execute();
    }
}