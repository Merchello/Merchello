namespace Merchello.Core.Chains
{
    using Umbraco.Core;

    /// <summary>
    /// Represents a PipelineTaskHandler
    /// </summary>
    /// <typeparam name="T">The type of value passed in the chain</typeparam>
    public class AttemptChainTaskHandler<T> : IAttemptChainTaskHandler<T>
    {
        /// <summary>
        /// The task.
        /// </summary>
        private readonly IAttemptChainTask<T> _task;

        /// <summary>
        /// The next.
        /// </summary>
        private IAttemptChainTaskHandler<T> _next;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttemptChainTaskHandler{T}"/> class.
        /// </summary>
        /// <param name="task">
        /// The task.
        /// </param>
        public AttemptChainTaskHandler(IAttemptChainTask<T> task)
        {
            Ensure.ParameterNotNull(task, "task");

            _task = task;

            // set the default next to the end of chain task
            _next = AttemptChainEndOfChainHandler<T>.Instance;
        }

        /// <summary>
        /// Attempt to execute the task.  If successful, executes the next task until.  This process is repeated until
        /// the end of chain Task is reached.
        /// </summary>
        /// <param name="arg">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public virtual Attempt<T> Execute(T arg)
        {
            var attempt = _task.PerformTask(arg);
            return attempt.Success
                ? _next.Execute(attempt.Result)
                : attempt;
        }

        /// <summary>
        /// The register next.
        /// </summary>
        /// <param name="next">
        /// The next.
        /// </param>
        public virtual void RegisterNext(IAttemptChainTaskHandler<T> next)
        {
            _next = next;
        }
    }
}
