namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The shipment status.
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ShipmentStatus : NotifiedStatusBase, IShipmentStatus
    {
    }
}