namespace Merchello.Core.Configuration.Outline
{
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// The type field definitions element.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public class TypeFieldDefinitionsElement : ConfigurationElement
    {
        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="ICustomerAddress"/>
        /// </summary>
        [ConfigurationProperty("customerAddress", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        internal TypeFieldCollection CustomerAddress
        {
            get { return (TypeFieldCollection)this["customerAddress"]; }
        }

        /// <summary>
        /// Gets the campaign offers.
        /// </summary>
        [ConfigurationProperty("campaignActivities", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection CampaignActivities
        {
            get
            {
                return (TypeFieldCollection)this["campaignActivities"];
            }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IItemCache"/>
        /// </summary>
        [ConfigurationProperty("itemCache", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection ItemCache
        {
            get { return (TypeFieldCollection)this["itemCache"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for Entities
        /// </summary>
        [ConfigurationProperty("entities", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        internal TypeFieldCollection Entities
        {
            get { return (TypeFieldCollection) this["entities"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IShipMethod"/>
        /// </summary>
        [ConfigurationProperty("shipMethod", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        internal TypeFieldCollection ShipMethod
        {
            get { return (TypeFieldCollection)this["shipMethod"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IInvoiceLineItem"/>
        /// </summary>
        [ConfigurationProperty("invoiceItem", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        internal TypeFieldCollection InvoiceItem
        {
            get { return (TypeFieldCollection)this["invoiceItem"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IPayment"/>
        /// </summary>
        [ConfigurationProperty("paymentMethod", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        internal TypeFieldCollection PaymentMethod
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
        internal TypeFieldCollection AppliedPayment
        {
            get { return (TypeFieldCollection)this["appliedPayment"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for <see cref="IGatewayProviderSettings"/>
        /// </summary>
        [ConfigurationProperty("gatewayProvider", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        internal TypeFieldCollection GatewayProvider
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
