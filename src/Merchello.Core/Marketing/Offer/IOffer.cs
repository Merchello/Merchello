namespace Merchello.Core.Marketing.Offer
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Marker interface for an offer.
    /// </summary>
    public interface IOffer
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        /// <remarks>
        /// This is actually a reference to the OfferSettings key
        /// </remarks>
        Guid Key { get; }

        /// <summary>
        /// Gets or sets the offer provider key.
        /// </summary>
        Guid OfferProviderKey { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the offer code.
        /// </summary>
        string OfferCode { get; set; }

        /// <summary>
        /// Gets or sets the offer starts date.
        /// </summary>
        DateTime OfferStartsDate { get; set; }

        /// <summary>
        /// Gets or sets the offer ends date.
        /// </summary>
        DateTime OfferEndsDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the offer is active.
        /// </summary>
        bool Active { get; set; }

        /// <summary>
        /// Attempts to apply the constraints against the offer.
        /// </summary>
        /// <param name="validatedAgainst">
        /// The validated against.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<IOfferResult<TConstraint, TAward>> TryApplyConstraints<TConstraint, TAward>(object validatedAgainst, ICustomerBase customer)
            where TConstraint : class
            where TAward : class;

        /// <summary>
        /// Attempts to award the reward defined by the offer
        /// </summary>
        /// <param name="validatedAgainst">
        /// An object passed to the offer constraints.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <param name="applyConstraints">
        /// Optional parameter indicating whether or not to apply constraints before attempting to award the reward.
        /// Defaults to true.
        /// </param>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <returns>
        /// The <see cref="Attempt{IOfferResult}"/>.
        /// </returns>
        Attempt<IOfferResult<TConstraint, TAward>> TryToAward<TConstraint, TAward>(object validatedAgainst, ICustomerBase customer, bool applyConstraints = true)             
            where TConstraint : class
            where TAward : class;
    }
}