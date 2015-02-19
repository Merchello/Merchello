namespace Merchello.Bazaar.Models
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Models;

    /// <summary>
    /// Defines the master model.
    /// </summary>
    public interface IMasterModel
    {
        /// <summary>
        /// Gets the theme.
        /// </summary>
        string Theme { get; }

        /// <summary>
        /// Gets or sets the store title.
        /// </summary>
        string StoreTitle { get; set; }

        /// <summary>
        /// Gets or sets the current customer.
        /// </summary>
        ICustomerBase CurrentCustomer { get; set; }

        /// <summary>
        /// Gets the currency.
        /// </summary>
        ICurrency Currency { get; }

        /// <summary>
        /// Gets the store page.
        /// </summary>
        StoreModel StorePage { get; }

        /// <summary>
        /// Gets the basket page.
        /// </summary>
        IPublishedContent BasketPage { get; }

        /// <summary>
        /// Gets the product groups.
        /// </summary>
        IEnumerable<ProductGroupModel> ProductGroups { get; } 
    }
}