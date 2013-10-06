using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Merchello.Tests.Base.Prototyping.Models
{
    /// <summary>
    /// Defines a product option collection
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class OptionCollection : KeyedCollection<int, IOption>
    {
        protected override int GetKeyForItem(IOption item)
        {
            return item.Id;
        }
    }
}