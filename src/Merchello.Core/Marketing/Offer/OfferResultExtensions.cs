namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Umbraco.Core;

    /// <summary>
    /// Utility extensions methods for .
    /// </summary>
    public static class OfferResultExtensions
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
        internal static Attempt<IOfferResult<TConstraint, TAward>> As<TConstraint, TAward>(this Attempt<IOfferResult<object, object>> attempt)
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