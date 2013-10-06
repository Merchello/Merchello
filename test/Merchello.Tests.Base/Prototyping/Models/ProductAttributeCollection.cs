using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using NUnit.Framework;

namespace Merchello.Tests.Base.Prototyping.Models
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