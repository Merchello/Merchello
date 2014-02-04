using System;
using Umbraco.Core;

namespace Merchello.Core.Pipelines
{
    /// <summary>
    /// Represents an end of chain PipelineTaskHander.  This terminates the task chain.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class PipelineEndOfChainHandler<T> : IPipelineTaskHandler<T>

    {
        private static readonly PipelineEndOfChainHandler<T> _instance = new PipelineEndOfChainHandler<T>();

        public static PipelineEndOfChainHandler<T> Instance
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
        public void RegisterNext(IPipelineTaskHandler<T> next)
        {
            throw new InvalidOperationException("Cannot register next on the end of chain.");
        }
    }
}