namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Constraints;

    /// <summary>
    /// Defines the OfferComponentResolver.
    /// </summary>
    internal interface IOfferComponentResolver
    {
        /// <summary>
        /// Gets the collection of <see cref="OfferComponentBase"/> that can be associated with a provider.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{OfferComponentBase}"/>.
        /// </returns>
        IEnumerable<OfferComponentBase> GetOfferComponentsByProviderKey(Guid providerKey);
            
        ///// <summary>
        ///// Gets a <see cref="OfferComponentBase"/> by it's definition
        ///// </summary>
        ///// <typeparam name="T">
        ///// The type of component to return
        ///// </typeparam>
        ///// <param name="definition">
        ///// The <see cref="OfferComponentDefinition"/>.
        ///// </param>
        ///// <returns>
        ///// The <see cref="IOfferConstraintComponent"/>.
        ///// </returns>
        //T GetOfferComponent<T>(OfferComponentDefinition definition) where T : OfferComponentBase;

        /// <summary>
        /// Gets a <see cref="OfferComponentBase"/> by it's definition
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="OfferComponentBase"/>.
        /// </returns>
        OfferComponentBase GetOfferComponent(OfferComponentDefinition definition);

        /// <summary>
        /// Returns a collection of all resolved <see cref="IOfferConstraintComponent"/> given a collection of definition.
        /// </summary>
        /// <param name="definitions">
        /// The definition.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IOfferConstraintComponent}"/>.
        /// </returns>
        IEnumerable<OfferComponentBase> GetOfferComponents(IEnumerable<OfferComponentDefinition> definitions);            
    }
}