using System.Configuration;
using Merchello.Core.Models;


namespace Merchello.Core.Configuration.Outline
{
    public class TypeFieldDefinitionsElement : ConfigurationElement
    {

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="ICustomerAddress"/>
        /// </summary>
        [ConfigurationProperty("customerAddress", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection CustomerAddress
        {
            get { return (TypeFieldCollection)this["customerAddress"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IItemCache"/>
        /// </summary>
        [ConfigurationProperty("customerItemCache", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection CustomerItemCache
        {
            get { return (TypeFieldCollection)this["customerItemCache"]; }
        }

        /// <summary>
        /// Gets teh dbTypeFields configuration collection for Entities
        /// </summary>
        [ConfigurationProperty("entities", IsRequired =  false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection Entities
        {
            get { return (TypeFieldCollection) this["entities"]; }
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
        /// Gets the dbTypeFields configuration collection for <see cref="IInvoiceLineItem"/>
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
        /// Gets the dbTypeFields configuration collection for <see cref="IProductVariant"/>
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
        /// Gets the dbTypeFields configuration collection for <see cref="IGatewayProvider"/>
        /// </summary>
        [ConfigurationProperty("gatewayProvider", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection GatewayProvider
        {
            get { return (TypeFieldCollection)this["gatewayProvider"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="ILineItem"/>
        /// </summary>
        [ConfigurationProperty("lineItem", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection LineItem
        {
            get { return (TypeFieldCollection) this["lineItem"]; }
        }
    }
}
