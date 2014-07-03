namespace Merchello.Core.Chains
{
    using System;
    using Umbraco.Core;

    /// <summary>
    /// Represents an end of chain PipelineTaskHander.  This terminates the task chain.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class AttemptChainEndOfChainHandler<T> : IAttemptChainTaskHandler<T>
    {
        /// <summary>
        /// The instance.
        /// </summary>
        private static readonly AttemptChainEndOfChainHandler<T> _instance = new AttemptChainEndOfChainHandler<T>();

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static AttemptChainEndOfChainHandler<T> Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <param name="arg"></param>
        /// <returns><see cref="Attempt"/> of T</returns>
        public Attempt<T> Execute(T arg)
        {
            return Attempt<T>.Succeed(arg);
        }

        /// <summary>
        /// Registers the next task
        /// </summary>
        /// <param name="next"></param>
        public void RegisterNext(IAttemptChainTaskHandler<T> next)
        {
            throw new InvalidOperationException("Cannot register next on the end of chain.");
        }
    }
}