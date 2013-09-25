using System;
using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    internal sealed class GatewayProviderTypeField  : TypeFieldMapper<GatewayProviderType>, IGatewayProviderTypeField
    {

        internal GatewayProviderTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }


        internal override void BuildCache()
        {
            AddUpdateCache(GatewayProviderType.Payment, new TypeField("Payment", "Payment", new Guid("A0B4F835-D92E-4D17-8181-6C342C41606E")));
            AddUpdateCache(GatewayProviderType.Shipping, new TypeField("Shipping", "Shipping", new Guid("646D3EA7-3B31-45C1-9488-7C0449A564A6")));
            AddUpdateCache(GatewayProviderType.Taxation, new TypeField("Taxation", "Taxation", new Guid("360B47F9-A4FB-4B96-81B4-A4A497D2B44A")));
        }


        /// <summary>
        /// Indicates the gateway provider is for shipping
        /// </summary>
        public ITypeField Shipping
        {
            get { return GetTypeField(GatewayProviderType.Shipping); }
        }

        /// <summary>
        /// Indicates the gateway provider is for payments
        /// </summary>
        public ITypeField Payment
        {
            get { return GetTypeField(GatewayProviderType.Payment); }
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