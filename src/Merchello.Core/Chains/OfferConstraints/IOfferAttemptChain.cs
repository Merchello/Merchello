namespace Merchello.Core.Chains.OfferConstraints
{
    using System;

    using Merchello.Core.Chains.InvoiceCreation;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The OfferAttemptChain interface.
    /// </summary>
    public interface IOfferAttemptChain
    {
        /// <summary>
        /// Executes the task chain to apply the constraints
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
        Attempt<object> TryApplyConstraints(object validatedAgainst, ICustomerBase customer);

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
        Attempt<object> TryAward(object validatedAgainst, ICustomerBase customer);
    }
}