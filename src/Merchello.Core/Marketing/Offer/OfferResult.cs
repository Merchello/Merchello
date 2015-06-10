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
}