using System;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Marker interface for a consumer
    /// </summary>
    /// <remarks>
    /// Enables either an ICustomer or IAnonymousCustomer to be a parameter to the IBasket  
    /// </remarks>
    public interface IConsumer : IKeyEntity
    {
    }
}
