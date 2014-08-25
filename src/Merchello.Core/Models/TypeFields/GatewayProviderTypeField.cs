using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    internal sealed class GatewayProviderTypeField  : TypeFieldMapper<GatewayProviderType>, IGatewayProviderTypeField
    {

        internal GatewayProviderTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }


        internal override void BuildCache()
        {
            AddUpdateCache(GatewayProviderType.Payment, new TypeField("Payment", "Payment", Constants.TypeFieldKeys.GatewayProvider.PaymentProviderKey));
            AddUpdateCache(GatewayProviderType.Notification, new TypeField("Notification", "Notification", Constants.TypeFieldKeys.GatewayProvider.NotificationProviderKey));
            AddUpdateCache(GatewayProviderType.Shipping, new TypeField("Shipping", "Shipping", Constants.TypeFieldKeys.GatewayProvider.ShippingProviderKey));
            AddUpdateCache(GatewayProviderType.Taxation, new TypeField("Taxation", "Taxation", Constants.TypeFieldKeys.GatewayProvider.TaxationProviderKey));
        }

        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return GatewayProviders.GetTypeFields().Select(GetTypeField);
            }
        }


        /// <summary>
        /// Indicates the gateway provider is for payments
        /// </summary>
        public ITypeField Payment
        {
            get { return GetTypeField(GatewayProviderType.Payment); }
        }

        /// <summary>
        /// Indicates the gateway provider is for Notifications
        /// </summary>
        public ITypeField Notification 
        {
            get { return GetTypeField(GatewayProviderType.Notification); }
        }

        /// <summary>
        /// Indicates the gateway provider is for shipping
        /// </summary>
        public ITypeField Shipping
        {
            get { return GetTypeField(GatewayProviderType.Shipping); }
        }



        /// <summary>
        /// Indicates the gateway provider is for taxation
        /// </summary>
        public ITypeField Taxation
        {
            get { return GetTypeField(GatewayProviderType.Taxation); }
        }

        /// <summary>
        /// Returns a gateway provider typefield or NullTypeField TypeKey (Guid)
        /// </summary>
        /// <param name="alias">The alias of the custom type field</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(GatewayProviders[alias]);
        }


        private static TypeFieldCollection GatewayProviders
        {
            get { return Fields.GatewayProvider; }
        }
       
    }
}