namespace Merchello.Core.Checkout
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    /// <summary>
    /// Defines a checkout customer manager.
    /// </summary>
    public interface ICheckoutCustomerManager : ICheckoutContextManagerBase
    {
        /// <summary>
        /// Saves the bill to address
        /// </summary>
        /// <param name="billToAddress">The billing <see cref="IAddress"/></param>
        void SaveBillToAddress(IAddress billToAddress);

        /// <summary>
        /// Saves the ship to address
        /// </summary>
        /// <param name="shipToAddress">The shipping <see cref="IAddress"/></param>
        void SaveShipToAddress(IAddress shipToAddress);

        /// <summary>
        /// Gets the bill to address
        /// </summary>
        /// <returns>Return the billing <see cref="IAddress"/></returns>
        IAddress GetBillToAddress();

        /// <summary>
        /// Gets the bill to address
        /// </summary>
        /// <returns>Return the billing <see cref="IAddress"/></returns>
        IAddress GetShipToAddress();
    }
}