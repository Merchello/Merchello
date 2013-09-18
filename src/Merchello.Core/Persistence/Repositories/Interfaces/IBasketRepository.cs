using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the basket repository
    /// </summary>
    public interface IBasketRepository : IRepositoryQueryable<int, IBasket>
    {
        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        /// <param name="consumer"><see cref="IConsumer"/></param>
        /// <param name="basketType"><see cref="BasketType"/></param>
        /// <returns><see cref="IBasket"/></returns>
        /// <remarks>
        /// This method should not be used for custom BasketType.Custom
        /// </remarks>
        IBasket GetByConsumer(IConsumer consumer, BasketType basketType);

        /// <summary>
        /// Returns the consumer's basket of a given type
        /// </summary>
        /// <param name="consumer"><see cref="IConsumer"/></param>
        /// <param name="basketTypeKey"><see cref="ITypeField"/>.TypeKey</param>
        /// <returns><see cref="IBasket"/></returns>
        /// <remarks>
        /// Public use of this method is intended to access BasketType.Custom records
        /// </remarks>
        IBasket GetByConsumer(IConsumer consumer, Guid basketTypeKey);


        /// <summary>
        /// Returns a collection of all baskets associated with the consumer passed
        /// </summary>
        IEnumerable<IBasket> GetByConsumer(IConsumer consumer);
    }
}
