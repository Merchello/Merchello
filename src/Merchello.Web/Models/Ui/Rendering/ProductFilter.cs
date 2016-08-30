namespace Merchello.Web.Models.Ui.Rendering
{
    using Merchello.Core.Models.Interfaces;

    internal class ProductFilter : EntityCollectionProxyBase, IProductFilter
    {
        public ProductFilter(IEntityCollection collection)
            : base(collection)
        {
        }
    }
}