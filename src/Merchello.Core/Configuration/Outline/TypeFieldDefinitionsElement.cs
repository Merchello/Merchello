using System.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Models.GatewayProviders;


namespace Merchello.Core.Configuration.Outline
{
    public class TypeFieldDefinitionsElement : ConfigurationElement
    {

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IAddress"/>
        /// </summary>
        [ConfigurationProperty("customerAddress", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection CustomerAddress
        {
            get { return (TypeFieldCollection)this["customerAddress"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IBasket"/>
        /// </summary>
        [ConfigurationProperty("basket", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection Basket
        {
            get { return (TypeFieldCollection)this["basket"]; }
        }


        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IShipMethod"/>
        /// </summary>
        [ConfigurationProperty("shipMethod", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection ShipMethod
        {
            get { return (TypeFieldCollection)this["shipMethod"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IInvoiceItem"/>
        /// </summary>
        [ConfigurationProperty("invoiceItem", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection InvoiceItem
        {
            get { return (TypeFieldCollection)this["invoiceItem"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IPayment"/>
        /// </summary>
        [ConfigurationProperty("paymentMethod", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection PaymentMethod
        {
            get { return (TypeFieldCollection)this["paymentMethod"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IProductActual"/>
        /// </summary>
        [ConfigurationProperty("product", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection Product
        {
            get { return (TypeFieldCollection)this["product"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IAppliedPayment"/>
        /// </summary>
        [ConfigurationProperty("appliedPayment", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection AppliedPayment
        {
            get { return (TypeFieldCollection)this["appliedPayment"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IRegisteredGatewayProviderBase"/>
        /// </summary>
        [ConfigurationProperty("gatewayProvider", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection GatewayProvider
        {
            get { return (TypeFieldCollection)this["gatewayProvider"]; }
        }

    }
}
