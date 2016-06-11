namespace Merchello.Core.Chains.OfferConstraints
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Exceptions;
    using Merchello.Core.Logging;
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
    public abstract class OfferProcessorBase<TConstraint, TAward> : IOfferProcessor
    {
        /// <summary>
        /// The _task handlers.
        /// </summary>
        private readonly List<AttemptChainTaskHandler<TConstraint>> _taskHandlers = new List<AttemptChainTaskHandler<TConstraint>>();

        /// <summary>
        /// The constraints.
        /// </summary>
        private IEnumerable<OfferConstraintComponentBase<TConstraint>> _constraints;

        /// <summary>
        /// The reward.
        /// </summary>
        private OfferRewardComponentBase _reward;

        /// <summary>
        /// Gets a value indicating whether is initialized.
        /// </summary>
        public bool IsInitialized
        {
            get { return _constraints != null && _reward != null; }
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
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<object> TryApplyConstraints(object validatedAgainst, ICustomerBase customer)
        {
            if (!this.IsInitialized) return Attempt<object>.Fail(new OfferRedemptionException("Offer processor not initialized."));

            var convert = validatedAgainst.TryConvertTo<TConstraint>();
            if (!convert.Success)
            {
                var invalid =
                    new InvalidOperationException(
                        "validatedAgainst parameter could not be converted to type " + typeof(TConstraint).FullName);
                return Attempt<object>.Fail(invalid);
            }

            if (TaskHandlers.Any()) TaskHandlers.Clear();
            this.BuildConstraintChain(this._constraints.Select(x => this.ConvertConstraintToTask(x, customer)));

            var attempt = TaskHandlers.Any()
                              ? TaskHandlers.First().Execute(convert.Result)
                              : Attempt<TConstraint>.Succeed(convert.Result); // there were no constraints

            // convert bask to an object
            return attempt.Success
                       ? Attempt<object>.Succeed(attempt.Result)
                       : Attempt<object>.Fail(attempt.Result, attempt.Exception);
        }

        /// <summary>
        /// Try to apply the award
        /// </summary>
        /// <param name="validatedAgainst">
        /// The validated against.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public Attempt<object> TryAward(object validatedAgainst, ICustomerBase customer)
        {
            if (!this.IsInitialized) return Attempt<object>.Fail(new OfferRedemptionException("Offer processor not initialized."));

            try
            {
                var reward = _reward as OfferRewardComponentBase<TConstraint, TAward>;
                var converted = validatedAgainst.TryConvertTo<TConstraint>();
                if (converted.Success)
                {
                    if (reward == null) return Attempt<object>.Fail(new NullReferenceException("Converted reward was null"));

                    var rewardAttempt = reward.TryAward(converted.Result, customer);

                    return !rewardAttempt.Success ? 
                               Attempt<object>.Fail(rewardAttempt.Result, rewardAttempt.Exception) : 
                               Attempt<object>.Succeed(rewardAttempt.Result);
                }

                MultiLogHelper.Error(typeof(OfferProcessorBase<TConstraint, TAward>), "Failed to convert validation object", converted.Exception);
                throw converted.Exception;
            }
            catch (Exception ex)
            {             
                MultiLogHelper.Error(typeof(OfferProcessorBase<TConstraint, TAward>), "Failed to convert reward type", ex);
                throw;                   
            }                           
        }


        /// <summary>
        /// Initializes the processor.
        /// </summary>
        /// <param name="constraints">
        /// The constraints.
        /// </param>
        /// <param name="reward">
        /// The reward.
        /// </param>
        /// <remarks>
        /// We need this for because we don't know the types upfront
        /// </remarks>
        public void Initialize(IEnumerable<OfferConstraintComponentBase> constraints, OfferRewardComponentBase reward)
        {
            var converted = new List<OfferConstraintComponentBase<TConstraint>>();
            try
            {
                converted.AddRange(constraints.ToArray().Select(constraint => constraint as OfferConstraintComponentBase<TConstraint>));
            }
            catch (Exception ex)
            {
                MultiLogHelper.Error<OfferProcessorBase<TConstraint, TAward>>("Failed to convert offer constraint to typed version.", ex);
                throw;
            }


            _constraints = converted;
            _reward = reward;
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