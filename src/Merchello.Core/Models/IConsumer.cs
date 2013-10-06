using System;
using Merchello.Core.Models.EntityBase;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a consumer
    /// </summary>
    /// <remarks>
    /// Enables either an ICustomer or IAnonymousCustomer to be a valid reference for the IConsumerRegister  
    /// </remarks>
    public interface IConsumer : IKeyEntity
    {

        bool IsAnonymous { get; }
    }
}
