namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents an order status.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class OrderStatus : NotifiedStatusBase, IOrderStatus
    {
    }
}