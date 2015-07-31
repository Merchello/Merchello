namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Utility class to for converting type fields.
    /// </summary>
    public class EnumTypeFieldConverter
    {
        /// <summary>
        /// Gets the <see cref="IAddressTypeField"/>
        /// </summary>
        public static IAddressTypeField Address
        {
            get { return new AddressTypeField(); } 
        }


        /// <summary>
        /// Gets the <see cref="IItemCacheTypeField"/>
        /// </summary>
        public static IItemCacheTypeField ItemItemCache
        {
            get { return new ItemCacheTypeField(); }            
        }

        /// <summary>
        /// Gets the <see cref="ILineItemTypeField"/>
        /// </summary>
        public static ILineItemTypeField LineItemType
        {
            get { return new LineItemTypeField(); }
        }
      
        /// <summary>
        /// Gets the <see cref="IPaymentMethodTypeField"/>
        /// </summary>
        public static IPaymentMethodTypeField PaymentMethod
        {
            get { return new PaymentMethodTypeField(); }
        }

        /// <summary>
        /// Gets the <see cref="IAppliedPaymentTypeField"/>
        /// </summary>
        /// <returns></returns>
        public static IAppliedPaymentTypeField AppliedPayment
        {
           get { return new AppliedPaymentTypeField(); }
        }

        /// <summary>
        /// Gets the <see cref="IAppliedPaymentTypeField"/>
        /// </summary>
        /// <returns></returns>
        public static IProductTypeField Product
        {
            get { return new ProductTypeField(); }
        }

        /// <summary>
        /// Gets the <see cref="IEntityTypeField"/>
        /// </summary>
        internal static IEntityTypeField EntityType
        {
            get { return new EntityTypeField(); }
        }

        /// <summary>
        /// Gets the <see cref="IGatewayProviderTypeField"/>
        /// </summary>
        internal static IGatewayProviderTypeField GatewayProvider
        {
            get { return new GatewayProviderTypeField(); }
        }
    }
}
