namespace Merchello.Core.Builders
{
    using System.Collections.Generic;
    using System.Linq;
    using Chains;
    using Umbraco.Core;

    /// <summary>
    /// Represents the build chain base class
    /// </summary>
    /// <typeparam name="T"><see cref="Attempt"/> of T</typeparam>
    public abstract class BuildChainBase<T> : IBuilderChain<T>
    {
        /// <summary>
        /// The _task handlers.
        /// </summary>
        private readonly List<AttemptChainTaskHandler<T>> _taskHandlers = new List<AttemptChainTaskHandler<T>>();

        /// <summary>
        /// Constructs the task chain
        /// </summary>
        /// <param name="chainConfigurationAlias">
        /// The chain Configuration Alias.
        /// </param>
        protected virtual void ResolveChain(string chainConfigurationAlias)
        {            
            // Types from the merchello.config file
            var typeList = ChainTaskResolver.GetTypesForChain(chainConfigurationAlias).ToArray();
            if (!typeList.Any()) return;

            // instantiate each task in the chain
            TaskHandlers.AddRange(
                typeList.Select(
                typeName => new AttemptChainTaskHandler<T>(
                    ActivatorHelper.CreateInstance<AttemptChainTaskBase<T>>(
                        typeName, 
                        ConstructorArgumentValues.ToArray()).Result)));

            // register the next task for each link (these are linear chains)
            foreach (var taskHandler in TaskHandlers.Where(task => TaskHandlers.IndexOf(task) != TaskHandlers.IndexOf(TaskHandlers.Last())))
            {
                taskHandler.RegisterNext(TaskHandlers[TaskHandlers.IndexOf(taskHandler) + 1]);
            }
        }


        /// <summary>
        /// Performs the "build" work
        /// </summary>
        /// <returns><see cref="Attempt"/> of T</returns>
        public abstract Attempt<T> Build();

        /// <summary>
        /// Defines the arguements required by the task's constructors to instantiate the chain
        /// </summary>
        protected abstract IEnumerable<object> ConstructorArgumentValues { get; } 

        /// <summary>
        /// Gets the list of task handlers
        /// </summary>
        protected List<AttemptChainTaskHandler<T>> TaskHandlers
        {
            get { return _taskHandlers; }
        }
    }
}