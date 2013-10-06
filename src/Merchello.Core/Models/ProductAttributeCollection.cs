using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a product attribute collection
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class ProductAttributeCollection : KeyedCollection<int, IProductAttribute>
    {
        protected override int GetKeyForItem(IProductAttribute item)
        {
            return item.Id;
        }
    }
}