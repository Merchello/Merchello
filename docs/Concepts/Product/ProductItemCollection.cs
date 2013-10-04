using System.Collections.ObjectModel;

namespace Merchello.Tests.Base.Prototyping
{
    public class ProductItemCollection : KeyedCollection<string, IProductItem>
    {
        protected override string GetKeyForItem(IProductItem item)
        {
            return item.Sku;
        }
    }
}