namespace Merchello.Core.Marketing.Constraints
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A base class for offer constraints
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to return
    /// </typeparam>
    public abstract class OfferConstraintComponentBase<T> : OfferConstraintComponentBase, IOfferConstraintComponent<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferConstraintComponentBase{T}"/> class. 
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        protected OfferConstraintComponentBase(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets the Type this component is responsible for building
        /// </summary>
        internal override Type BuilderFor
        {
            get
            {
                return typeof(T);
            }
        }

        /// <summary>
        /// Validates the constraint against the <see cref="ILineItemContainer"/>
        /// </summary>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{ILineItemContainer}"/> indicating whether or not the constraint can be enforced.
        /// </returns>
        public abstract Attempt<T> Validate(ICustomerBase customer, ILineItemContainer collection);
    }
}