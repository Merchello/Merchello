namespace Merchello.Core.Models.TypeFields
{
    /// <inheritdoc/>
    internal sealed class GatewayProviderTypeField : TypeFieldMapper<GatewayProviderType>, IGatewayProviderTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GatewayProviderTypeField"/> class.
        /// </summary>
        internal GatewayProviderTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <inheritdoc/>
        public ITypeField Payment
        {
            get
            {
                return GetTypeField(GatewayProviderType.Payment);
            }
        }

        /// <inheritdoc/>
        public ITypeField Notification 
        {
            get
            {
                return GetTypeField(GatewayProviderType.Notification);
            }
        }

        /// <inheritdoc/>
        public ITypeField Shipping
        {
            get
            {
                return GetTypeField(GatewayProviderType.Shipping);
            }
        }



        /// <inheritdoc/>
        public ITypeField Taxation
        {
            get
            {
                return GetTypeField(GatewayProviderType.Taxation);
            }
        }

        /// <inheritdoc/>
        internal override void BuildCache()
        {
            AddUpdateCache(GatewayProviderType.Payment, new TypeField("Payment", "Payment", Constants.TypeFieldKeys.GatewayProvider.PaymentProviderKey));
            AddUpdateCache(GatewayProviderType.Notification, new TypeField("Notification", "Notification", Constants.TypeFieldKeys.GatewayProvider.NotificationProviderKey));
            AddUpdateCache(GatewayProviderType.Shipping, new TypeField("Shipping", "Shipping", Constants.TypeFieldKeys.GatewayProvider.ShippingProviderKey));
            AddUpdateCache(GatewayProviderType.Taxation, new TypeField("Taxation", "Taxation", Constants.TypeFieldKeys.GatewayProvider.TaxationProviderKey));
        }
    }
}