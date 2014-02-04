using Umbraco.Core;

namespace Merchello.Core.Pipelines
{
    internal abstract class PipelineTaskHandlerBase<T> : IPipelineTaskHandler<T>
    {
        private readonly IPipelineTask<T> _task;
        private IPipelineTaskHandler<T> _next; 

        protected PipelineTaskHandlerBase(IPipelineTask<T> task)
        {
            Mandate.ParameterNotNull(task, "task");

            _task = task;
            _next = PipelineEndOfChainHandler<T>.Instance;
        }


        public Attempt<T> Execute(T arg)
        {
            var attempt = _task.PerformTask(arg);
            return attempt.Success
                       ? _next.Execute(attempt.Result)
                       : attempt;
        }

        public virtual void RegisterNext(IPipelineTaskHandler<T> next)
        {
            _next = next;
        }
    }
}