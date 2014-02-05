namespace Merchello.Core.Pipelines
{
    internal interface IPipelineBase<T>
    {
        int TaskCount { get; }

        T Execute(T arg);
    }
}