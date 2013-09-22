using System;
using Merchello.Core.OrderFulfillment;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines an order fulfillment provider
    /// </summary>
    public interface IFulfillmentProvider : IOrderFulfillment
    {
        /// <summary>
        /// The unique 
        /// </summary>
        Guid ProviderKey { get; set; }
    }
}
