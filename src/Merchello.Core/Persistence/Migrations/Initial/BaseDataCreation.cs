using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence.Migrations.Initial
{
    /// <summary>
    /// Represents the initial data creation by running Insert for the base data.
    /// </summary>
    internal class BaseDataCreation
    {
        private readonly Database _database;

        public BaseDataCreation(Database database)
        {
            _database = database;
        }

        /// <summary>
        /// Initialize the base data creation by inserting the data foundation for umbraco
        /// specific to a table
        /// </summary>
        /// <param name="tableName">Name of the table to create base data for</param>
        public void InitializeBaseData(string tableName)
        {
            LogHelper.Info<BaseDataCreation>(string.Format("Creating data in table {0}", tableName));

            if (tableName.Equals("merchDBTypeField"))
            {
                CreateDbTypeFieldData();
            }
        }

        private void CreateDbTypeFieldData()
        {
            // AddressTypeField
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = AddressTypeField.Residential.TypeKey, Alias = AddressTypeField.Residential.Alias, Name = AddressTypeField.Residential.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = AddressTypeField.Commercial.TypeKey, Alias = AddressTypeField.Commercial.Alias, Name = AddressTypeField.Commercial.Name });

            // BasketTypeField
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = BasketTypeField.Basket.TypeKey, Alias = BasketTypeField.Basket.Alias, Name = BasketTypeField.Basket.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = BasketTypeField.Wishlist.TypeKey, Alias = BasketTypeField.Wishlist.Alias, Name = BasketTypeField.Wishlist.Name });

            // InvoiceItemType
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = InvoiceItemTypeField.Product.TypeKey, Alias = InvoiceItemTypeField.Product.Alias, Name = InvoiceItemTypeField.Product.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = InvoiceItemTypeField.Charge.TypeKey, Alias = InvoiceItemTypeField.Charge.Alias, Name = InvoiceItemTypeField.Charge.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = InvoiceItemTypeField.Credit.TypeKey, Alias = InvoiceItemTypeField.Credit.Alias, Name = InvoiceItemTypeField.Credit.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = InvoiceItemTypeField.Shipping.TypeKey, Alias = InvoiceItemTypeField.Shipping.Alias, Name = InvoiceItemTypeField.Shipping.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = InvoiceItemTypeField.Tax.TypeKey, Alias = InvoiceItemTypeField.Tax.Alias, Name = InvoiceItemTypeField.Tax.Name });

            // PaymentMethodType
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = PaymentMethodTypeField.Cash.TypeKey, Alias = PaymentMethodTypeField.Cash.Alias, Name = PaymentMethodTypeField.Cash.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = PaymentMethodTypeField.CreditCard.TypeKey, Alias = PaymentMethodTypeField.CreditCard.Alias, Name = PaymentMethodTypeField.CreditCard.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = PaymentMethodTypeField.PurchaseOrder.TypeKey, Alias = PaymentMethodTypeField.PurchaseOrder.Alias, Name = PaymentMethodTypeField.PurchaseOrder.Name });

            // ShipmentMethodType
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = ShipMethodTypeField.FlatRate.TypeKey, Alias = ShipMethodTypeField.FlatRate.Alias, Name = ShipMethodTypeField.FlatRate.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = ShipMethodTypeField.Carrier.TypeKey, Alias = ShipMethodTypeField.Carrier.Alias, Name = ShipMethodTypeField.Carrier.Name });
            _database.Insert("merchDBTypeField", "pk", new TypeFieldDto() { Pk = ShipMethodTypeField.PercentTotal.TypeKey, Alias = ShipMethodTypeField.PercentTotal.Alias, Name = ShipMethodTypeField.PercentTotal.Name });
        }
    }
}
