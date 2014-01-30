using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;

namespace Merchello.Core.Checkout
{
    /// <summary>
    /// Defines a Checkout base class
    /// </summary>
    public interface ICheckoutBase
    {
        /// <summary>
        /// Starts the checkout process over
        /// </summary>
        void StartOver();

        void RegisterBillToAddress(IAddress billToAddress);

        void RegisterShipments(IEnumerable<IShipment> shipments);

    }
}