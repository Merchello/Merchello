namespace Merchello.Core.Models.TypeFields
{
    /// <inheritdoc/>
    internal sealed class ProductTypeField : ExtendedTypeFieldMapper<ProductType>, IProductTypeField
    {
        /// <inheritdoc/>
        internal override void BuildCache()
        {
            AddUpdateCache(ProductType.Custom, NotFound);
        }
    }
}
