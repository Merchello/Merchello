namespace Merchello.Core.Marketing.Offer
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The offer reward result.
    /// </summary>
    /// <typeparam name="TConstraint">
    /// The type of constraint
    /// </typeparam>
    /// <typeparam name="TAward">
    /// The type of Award
    /// </typeparam>
    public class OfferResult<TConstraint, TAward> : IOfferResult<TConstraint, TAward> 
        where TConstraint : class
        where TAward : class
    {
        /// <summary>
        /// Gets or sets the award.
        /// </summary>
        public TAward Award { get; set; }

        /// <summary>
        /// Gets or sets the validated against.
        /// </summary>
        public TConstraint ValidatedAgainst { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        public ICustomerBase Customer { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public List<string> Messages { get; set; }
    }

    /// <summary>
    /// Utility extension to map an attempt of IOfferResult{object} to an attempt of IOfferResult{T}
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class OfferAwardResultExtextions
    {
        /// <summary>
        /// Map an attempt of IOfferResult{object} to an attempt of IOfferResult{T}
        /// </summary>
        /// <param name="attempt">
        /// The attempt.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of Award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public static Attempt<IOfferResult<TConstraint, TAward>> As<TConstraint, TAward>(this Attempt<IOfferResult<object, object>> attempt) 
            where TConstraint : class
            where TAward : class 
        {
            if (!attempt.Success)
            {
                var failed = Attempt<IOfferResult<TConstraint, TAward>>.Fail(new OfferResult<TConstraint, TAward>(), attempt.Exception);
                if (attempt.Result != null)
                {
                    failed.Result.Customer = attempt.Result.Customer;
                    failed.Result.Messages = attempt.Result.Messages;
                }

                return failed;
            }

            var success = Attempt<IOfferResult<TConstraint, TAward>>.Succeed(new OfferResult<TConstraint, TAward>());
            success.Result.Award = attempt.Result.Award as TAward;
            success.Result.ValidatedAgainst = attempt.Result.ValidatedAgainst as TConstraint;
            success.Result.Customer = attempt.Result.Customer;
            success.Result.Messages = attempt.Result.Messages;
            return success;
        } 
    }
}