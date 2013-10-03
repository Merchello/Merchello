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


        internal OrderLineItem(int containerId, Guid lineItemTfKey)
            : this(containerId, lineItemTfKey, new LineItemCollection())
        { }

        internal OrderLineItem(int containerId, Guid lineItemTfKey, LineItemCollection itemization)
            : base(containerId, lineItemTfKey, itemization)
        { }

       
    }

}