namespace Merchello.Core.Chains
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The configuration chain base.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object the chain deals with
    /// </typeparam>
    public abstract class ConfigurationChainBase<T>
    {
        /// <summary>
        /// The _task handlers.
        /// </summary>
        private readonly List<AttemptChainTaskHandler<T>> _taskHandlers = new List<AttemptChainTaskHandler<T>>();

        /// <summary>
        /// Gets the arguments required by the task's constructors to instantiate the chain
        /// </summary>
        protected abstract IEnumerable<object> ConstructorArgumentValues { get; }

        /// <summary>
        /// Gets the list of task handlers
        /// </summary>
        protected List<AttemptChainTaskHandler<T>> TaskHandlers
        {
            get { return _taskHandlers; }
        }

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
    }
}