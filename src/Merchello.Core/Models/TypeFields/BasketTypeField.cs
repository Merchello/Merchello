using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Indicates whether a shopping cart basket is either a "basket" or a "wishlist" representation
    /// </summary>
    public class BasketTypeField : TypeFieldProxyBase
    {
        /// <summary>
        /// Default ecommerce basket
        /// </summary>
        public static ITypeField Basket
        {
            get { return Constants.BasketType.Basket; }
        }

        /// <summary>
        /// Wishlist
        /// </summary>
        public static ITypeField Wishlist
        {
            get { return Constants.BasketType.Wishlist; }
        }

        /// <summary>
        /// Returns a custom basket or a NullTypeField
        /// </summary>
        /// <param name="alias">The alias of the custom basket</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        public new static ITypeField Custom(string alias)
        {
            return GetTypeField(Baskets[alias]);
        }

        private static TypeFieldCollection Baskets
        {
            get { return Fields.Basket; }
        }
    }
}
