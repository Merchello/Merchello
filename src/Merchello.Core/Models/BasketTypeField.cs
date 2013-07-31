using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Indicates whether a shopping cart basket is either a "basket" or a "wishlist" representation
    /// </summary>
    public class BasketTypeField : TypeFieldBase
    {
        /// <summary>
        /// Default ecommerce basket
        /// </summary>
        public static ITypeField Basket
        {
            get { return GetTypeField(Baskets["Basket"]); }
        }

        /// <summary>
        /// Wishlist
        /// </summary>
        public static ITypeField Wishlist
        {
            get { return GetTypeField(Baskets["Wishlist"]);  }
        }


        private static TypeFieldCollection Baskets
        {
            get { return Fields.Basket; }
        }
    }
}
