using System.Collections.ObjectModel;

namespace Merchello.Tests.Base.Prototyping
{
    public class ProductAttributeCollection : KeyedCollection<int, IProductAttribute>
    {
        protected override int GetKeyForItem(IProductAttribute item)
        {
            return item.Id;
        }
    }
}