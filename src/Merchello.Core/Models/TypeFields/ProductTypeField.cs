using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

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

        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return Products.GetTypeFields().Select(GetTypeField);
            }
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
