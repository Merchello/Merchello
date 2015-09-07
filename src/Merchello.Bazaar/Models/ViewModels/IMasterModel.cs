namespace Merchello.Bazaar.Models.ViewModels
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

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
        /// Gets a value indicating whether show account.
        /// </summary>
        bool ShowAccount { get; }

        /// <summary>
        /// Gets a value indicating whether to show the wish list.
        /// </summary>
        bool ShowWishList { get; }

        /// <summary>
        /// Gets the customer member type name.
        /// </summary>
        string CustomerMemberTypeName { get; }
    }
}