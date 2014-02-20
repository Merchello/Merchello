using Umbraco.Core;

namespace Merchello.Core.Chains
{
    /// <summary>
    /// Represents a PipelineTaskHandler
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AttemptChainTaskHandler<T> : IAttemptChainTaskHandler<T>
    {
        private readonly IAttemptChainTask<T> _task;
        private IAttemptChainTaskHandler<T> _next;

        public AttemptChainTaskHandler(IAttemptChainTask<T> task)
        {
            Mandate.ParameterNotNull(task, "task");

            _task = task;

            // set the default next to the end of chain task
            _next = AttemptChainEndOfChainHandler<T>.Instance;
        }

        /// <summary>
        /// Attempt to execute the task.  If successful, executes the next task until.  This process is repeated until
        /// the end of chain Task is reached.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public virtual Attempt<T> Execute(T arg)
        {
            var attempt = _task.PerformTask(arg);
            return attempt.Success
                ? _next.Execute(attempt.Result)
                : attempt;
        }

        /// <summary>
        /// Registers the next task in the chain.
        /// </summary>
        /// <param name="next"></param>
        public virtual void RegisterNext(IAttemptChainTaskHandler<T> next)
        {
            _next = next;
        }
    }
}
