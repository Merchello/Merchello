namespace Merchello.Web.Models.Ui
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Web.Models.VirtualContent;

    public class ProductContentList : IEnumerable<IProductContent>
    {
        private readonly IEnumerable<IProductContent> _products;

        public ProductContentList(IEnumerable<IProductContent> data)
        {
            _products = data ?? Enumerable.Empty<IProductContent>();
        }

        public IEnumerator<IProductContent> GetEnumerator()
        {
            return _products.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}