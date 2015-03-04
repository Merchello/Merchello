namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// The enum type field converter.
    /// </summary>
    public class EnumTypeFieldConverter
    {
        /// <summary>
        /// Creates an instance of an <see cref="IAddressTypeField"/> object
        /// </summary>
        public static IAddressTypeField Address
        {
            get { return new AddressTypeField(); } 
        }

        /// <summary>
        /// Creates an instance of an <see cref="IEntityTypeField"/> object
        /// </summary>
        internal static IEntityTypeField EntityType
        {
            get { return new EntityTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IItemCacheTypeField"/> object
        /// </summary>
        public static IItemCacheTypeField ItemItemCache
        {
            get { return new ItemCacheTypeField(); }            
        }

        /// <summary>
        /// Creates an instance of an <see cref="ILineItemTypeField"/> object
        /// </summary>
        /// <returns></returns>
        public static ILineItemTypeField LineItemType
        {
            get { return new LineItemTypeField(); }
        }
      
        /// <summary>
        /// Creates an instance of an <see cref="IPaymentMethodTypeField"/> object
        /// </summary>
        /// <returns></returns>
        public static IPaymentMethodTypeField PaymentMethod
        {
            get { return new PaymentMethodTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IAppliedPaymentTypeField"/>
        /// </summary>
        /// <returns></returns>
        public static IAppliedPaymentTypeField AppliedPayment
        {
           get { return new AppliedPaymentTypeField(); }
        }

        /// <summary>
        /// Creates an instance of an <see cref="IGatewayProviderTypeField"/>
        /// </summary>
        /// <returns></returns>
        internal static IGatewayProviderTypeField GatewayProvider
        {
            get { return new GatewayProviderTypeField(); }
        }
    }
}
