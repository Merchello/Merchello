using System;
using System.Collections.Generic;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a customer basket
    /// </summary>
    public interface IBasket : IIdEntity
    {
        /// <summary>
        /// The <see cref="IConsumer"/> key
        /// </summary>
        Guid ConsumerKey { get; set; }

        IEnumerable<IBasketItem> BasketItems { get; set; }

        /// <summary>
        /// Returns true or false indicating whether or not the basket is empty 
        /// </summary>
        bool IsEmpty();

        /// <summary>
        /// Returns the count of <see cref="IBasketItem"/>
        /// </summary>
        int BasketItemCount();
    }
}
