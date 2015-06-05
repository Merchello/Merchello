namespace Merchello.Core.Chains.OfferConstraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The offer constraint chain base.
    /// </summary>
    /// <typeparam name="TAward">
    /// The type of the award by parameter
    /// </typeparam>
    /// <typeparam name="TConstraint">
    /// The type of constraints
    /// </typeparam>
    public abstract class OfferAttemptChainBase<TConstraint, TAward> : IOfferAttemptChain
    {
        /// <summary>
        /// The _task handlers.
        /// </summary>
        private readonly List<AttemptChainTaskHandler<TConstraint>> _taskHandlers = new List<AttemptChainTaskHandler<TConstraint>>();

        /// <summary>
        /// The constraints.
        /// </summary>
        private readonly IEnumerable<OfferConstraintComponentBase<TConstraint>> _constraints;

        /// <summary>
        /// The reward.
        /// </summary>
        private readonly OfferRewardComponentBase _reward;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfferAttemptChainBase{TConstraint,TAward}"/> class. 
        /// </summary>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        /// <param name="reward">
        /// The reward.
        /// </param>
        protected OfferAttemptChainBase(IEnumerable<OfferConstraintComponentBase<TConstraint>> constraints, OfferRewardComponentBase reward)
        {
            this._constraints = constraints;
            this._reward = reward;
        }


        /// <summary>
        /// Gets the list of task handlers
        /// </summary>
        protected List<AttemptChainTaskHandler<TConstraint>> TaskHandlers
        {
            get { return _taskHandlers; }
        }


        /// <summary>
        /// Applies the constraints
        /// </summary>
        /// <param name="validatedAgainst">
        /// The constrain by.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<object> TryApplyConstraints(object validatedAgainst, ICustomerBase customer)
        {
            var convert = validatedAgainst.TryConvertTo<TConstraint>();
            if (!convert.Success)
            {
                var invalid =
                    new InvalidOperationException(
                        "validatedAgainst parameter could not be converted to type " + typeof(TConstraint).FullName);
            }

            if (TaskHandlers.Any()) TaskHandlers.Clear();
            this.BuildConstraintChain(this._constraints.Select(x => this.ConvertConstraintToTask(x, customer)));

            var attempt = TaskHandlers.Any()
                      ? TaskHandlers.First().Execute(convert.Result)
                      : Attempt<TConstraint>.Fail(new InvalidOperationException("The chain Task List could not be instantiated.  TaskHandlers array was empty."));

            // convert bask to an object
            return attempt.Success
                       ? Attempt<object>.Succeed(attempt.Result)
                       : Attempt<object>.Fail(attempt.Result, attempt.Exception);
        }

        public Attempt<object> TryAward(object validatedAgainst, ICustomerBase customer)
        {
            var attempt = _reward.TryConvertTo(typeof(OfferRewardComponentBase<TConstraint, TAward>));

            if (attempt.Success)
            {
                var reward = attempt.Result as OfferRewardComponentBase<TConstraint, TAward>;
                var converted = validatedAgainst.TryConvertTo<TConstraint>();
                if (converted.Success)
                {
                    if (reward != null)
                    {
                                           
                    var rewardAttempt = reward.TryAward(converted.Result, customer);

                    return !rewardAttempt.Success ? 
                        Attempt<object>.Fail(rewardAttempt.Result, rewardAttempt.Exception) : 
                        Attempt<object>.Succeed(rewardAttempt.Result);
                    }

                    return Attempt<object>.Fail(new NullReferenceException("Converted reward was null"));
                }

                LogHelper.Error(typeof(OfferAttemptChainBase<TConstraint, TAward>), "Failed to convert validation object", converted.Exception);
            }

            LogHelper.Error(typeof(OfferAttemptChainBase<TConstraint, TAward>), "Failed to convert reward type", attempt.Exception);
            throw attempt.Exception;
        }

        /// <summary>
        /// Convert constraint to task.
        /// </summary>
        /// <param name="constraint">
        /// The constraint.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="OfferConstraintChainTask{T}"/>.
        /// </returns>
        protected abstract OfferConstraintChainTask<TConstraint> ConvertConstraintToTask(OfferConstraintComponentBase<TConstraint> constraint, ICustomerBase customer);

        /// <summary>
        /// The build chain.
        /// </summary>
        /// <param name="tasks">
        /// The tasks.
        /// </param>
        private void BuildConstraintChain(IEnumerable<OfferConstraintChainTask<TConstraint>> tasks)
        {
            var constraintTasks = tasks as OfferConstraintChainTask<TConstraint>[] ?? tasks.ToArray();
            if (!constraintTasks.Any()) return;
            var handlers = constraintTasks.Select(x => new AttemptChainTaskHandler<TConstraint>(x));
            _taskHandlers.AddRange(handlers);

            // register the next task for each link (these are linear chains)
            foreach (var taskHandler in TaskHandlers.Where(task => TaskHandlers.IndexOf(task) != TaskHandlers.IndexOf(TaskHandlers.Last())))
            {
                taskHandler.RegisterNext(TaskHandlers[TaskHandlers.IndexOf(taskHandler) + 1]);
            }
        }
    }
}