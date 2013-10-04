using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Merchello.Tests.Base.Prototyping.Models
{
    /// <summary>
    /// Defines a product variant inventory collection
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class InventoryCollection : KeyedCollection<string, IInventory>
    {
        protected override string GetKeyForItem(IInventory item)
        {
            return string.Format("{0}-{1}", item.ProductVariantKey, item.WarehouseId);
        }
    }
}