using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Product Types
    /// </summary>
    internal  sealed class ProductTypeField: TypeFieldMapper<ProductType>, IProductTypeField
    {
        internal ProductTypeField()
        {
            //if(CachedTypeFields.IsEmpty) BuildCache();
        }

        internal override void BuildCache()
        {
        }

        /// <summary>
        /// Returns a product type or NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the product type</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(Products[alias]);
        }



        private static TypeFieldCollection Products
        {
            get { return Fields.Product; }
        }

    }

}
