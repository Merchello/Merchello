using System;
using System.Reflection;
using System.Runtime.Serialization;
using Merchello.Core.Models.TypeFields;

namespace Merchello.Core.Models
{

    /// <summary>
    /// Defines a purchase list item
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    internal class OrderLineItem : LineItemBase, IOrderLineItem
    {
        public OrderLineItem(int containerId, Guid lineItemTfKey)
            : base(containerId, lineItemTfKey)
        { }

    }

}