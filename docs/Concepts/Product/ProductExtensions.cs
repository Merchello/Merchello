using System.Linq;

namespace Merchello.Tests.Base.Prototyping
{
    public static class ProductExtensions
    {
        // TODO : This is super rough.  Needs testing
        public static IProductItem GetForAttributes(this IProduct product, int[] attributeIds)
        {
            var match = attributeIds.ToList().OrderBy(x => x);

            IProductItem pi = null;
            foreach (var item in product.ProductItems.Where(x => x.SingleItem == false))
            {
                var atts = (item.Attributes.Select(x => x.Id)).OrderBy(x => x);
                if (atts != match) continue;
                pi = item;
                break;
            }

            return pi;
        }
    }
}