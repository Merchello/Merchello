namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration.Outline;

    /// <summary>
    /// Product Types
    /// </summary>
    internal sealed class ProductTypeField : TypeFieldMapper<ProductType>, IProductTypeField
    {
        /// <summary>
        /// Gets the custom type fields.
        /// </summary>
        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return Products.GetTypeFields().Select(GetTypeField);
            }
        }

        /// <summary>
        /// Gets the products.
        /// </summary>
        private static TypeFieldCollection Products
        {
            get { return Fields.Product; }
        }

        /// <summary>
        /// The build cache.
        /// </summary>
        internal override void BuildCache()
        {
            AddUpdateCache(ProductType.Custom, NotFound);
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
    }
}
