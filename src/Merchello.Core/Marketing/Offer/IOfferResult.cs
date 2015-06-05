namespace Merchello.Core.Marketing.Offer
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// Defines an OfferResult.
    /// </summary>
    /// <typeparam name="TConstraint">
    /// The type of constraint
    /// </typeparam>
    /// <typeparam name="TAward">
    /// The type of award
    /// </typeparam>
    public interface IOfferResult<TConstraint, TAward>        
        where TConstraint : class
        where TAward : class
    {
        /// <summary>
        /// Gets or sets the award.
        /// </summary>
        TAward Award { get; set; }

        /// <summary>
        /// Gets or sets the validated against.
        /// </summary>
        TConstraint ValidatedAgainst { get; set; }

        /// <summary>
        /// Gets or sets the customer.
        /// </summary>
        ICustomerBase Customer { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        List<string> Messages { get; set; }
    }
}