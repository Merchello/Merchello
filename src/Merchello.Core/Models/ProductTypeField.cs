using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Product Types
    /// </summary>
    public class ProductTypeField: TypeFieldProxyBase
    {
        /// <summary>
        /// Returns a product type or NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the product type</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        public new static ITypeField Custom(string alias)
        {
            return GetTypeField(Products[alias]);
        }

        private static TypeFieldCollection Products
        {
            get { return Fields.Product; }
        }
    }

}
