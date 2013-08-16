using System.Configuration;


namespace Merchello.Core.Configuration.Outline
{
    public class TypeFieldDefinitionsElement : ConfigurationElement
    {

        /// <summary>
        /// Gets the dbTypeFields configuration collection for customer address
        /// </summary>
        [ConfigurationProperty("customerAddress", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection CustomerAddress
        {
            get { return (TypeFieldCollection)this["customerAddress"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for baskets
        /// </summary>
        [ConfigurationProperty("basket", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection Basket
        {
            get { return (TypeFieldCollection)this["basket"]; }
        }


        /// <summary>
        /// Gets the dbTypeFields configuration collection for shipping methods
        /// </summary>
        [ConfigurationProperty("shipMethod", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection ShipMethod
        {
            get { return (TypeFieldCollection)this["shipMethod"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for invoice items
        /// </summary>
        [ConfigurationProperty("invoiceItem", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection InvoiceItem
        {
            get { return (TypeFieldCollection)this["invoiceItem"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for payment type
        /// </summary>
        [ConfigurationProperty("paymentMethod", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection PaymentMethod
        {
            get { return (TypeFieldCollection)this["paymentMethod"]; }
        }

        /// <summary>
        /// Gets the dbTypeFields configuration collection for IProduct
        /// </summary>
        [ConfigurationProperty("product", IsRequired = false), ConfigurationCollection(typeof(TypeFieldCollection), AddItemName = "type")]
        public TypeFieldCollection Product
        {
            get { return (TypeFieldCollection)this["product"]; }
        }



    }
}
