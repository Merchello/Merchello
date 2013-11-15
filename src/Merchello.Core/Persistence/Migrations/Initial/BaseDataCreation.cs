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

            if (tableName.Equals("merchTypeField")) CreateDbTypeFieldData();

            if(tableName.Equals("merchInvoiceStatus")) CreateInvoiceStatusData();

            if(tableName.Equals("merchWarehouse")) CreateWarehouseData();
            
        }

        private void CreateDbTypeFieldData()
        {
            // address
            var address = new AddressTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = address.Residential.TypeKey, Alias = address.Residential.Alias, Name = address.Residential.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now});
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = address.Commercial.TypeKey, Alias = address.Commercial.Alias, Name = address.Commercial.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // BasketTypeField
            var basket = new CustomerItemCacheTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = basket.Basket.TypeKey, Alias = basket.Basket.Alias, Name = basket.Basket.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = basket.Wishlist.TypeKey, Alias = basket.Wishlist.Alias, Name = basket.Wishlist.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // InvoiceItemType
            var itf = new InvoiceItemTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itf.Item.TypeKey, Alias = itf.Item.Alias, Name = itf.Item.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itf.Charge.TypeKey, Alias = itf.Charge.Alias, Name = itf.Charge.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itf.Credit.TypeKey, Alias = itf.Credit.Alias, Name = itf.Credit.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itf.Shipping.TypeKey, Alias = itf.Shipping.Alias, Name = itf.Shipping.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itf.Tax.TypeKey, Alias = itf.Tax.Alias, Name = itf.Tax.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // PaymentMethodType
            var ptf = new PaymentMethodTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = ptf.Cash.TypeKey, Alias = ptf.Cash.Alias, Name = ptf.Cash.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = ptf.CreditCard.TypeKey, Alias = ptf.CreditCard.Alias, Name = ptf.CreditCard.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = ptf.PurchaseOrder.TypeKey, Alias = ptf.PurchaseOrder.Alias, Name = ptf.PurchaseOrder.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // ShipmentMethodType
            var stf = new ShipMethodTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = stf.FlatRate.TypeKey, Alias = stf.FlatRate.Alias, Name = stf.FlatRate.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = stf.Carrier.TypeKey, Alias = stf.Carrier.Alias, Name = stf.Carrier.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = stf.PercentTotal.TypeKey, Alias = stf.PercentTotal.Alias, Name = stf.PercentTotal.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // AppliedPaymentType
            var apf = new AppliedPaymentTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Debit.TypeKey, Alias = apf.Debit.Alias, Name = apf.Debit.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Credit.TypeKey, Alias = apf.Credit.Alias, Name = apf.Credit.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Void.TypeKey, Alias = apf.Void.Alias, Name = apf.Void.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // GatewayProviderType
            var gwp = new GatewayProviderTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Payment.TypeKey, Alias = gwp.Payment.Alias, Name = gwp.Payment.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Shipping.TypeKey, Alias = gwp.Shipping.Alias, Name = gwp.Shipping.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Taxation.TypeKey, Alias = gwp.Taxation.Alias, Name = gwp.Taxation.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
        }

        private void CreateInvoiceStatusData()
        {
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("17ADA9AC-C893-4C26-AA26-234ECEB2FA75"), Alias = "unpaid", Name = "Unpaid", Active = true, Reportable = true, SortOrder = 1, CreateDate = DateTime.Now, UpdateDate = DateTime.Now});
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("1F872A1A-F0DD-4C3E-80AB-99799A28606E"), Alias = "unshipped", Name = "Unshipped", Active = true, Reportable = true, SortOrder = 2, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("6606B0EA-15B6-44AA-8557-B2D9D049645C"), Alias = "completed", Name = "Completed", Active = true, Reportable = true, SortOrder = 3, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("53077EFD-6BF0-460D-9565-0E00567B5176"), Alias = "cancelled", Name = "Cancelled", Active = true, Reportable = true, SortOrder = 4, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("75E1E5EB-33E8-4904-A8E5-4B64A37D6087"), Alias = "fraud", Name = "Fraud", Active = true, Reportable = true, SortOrder = 5, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }


        private void CreateWarehouseData()
        {
            _database.Insert("merchWarehouse", "Key", new WarehouseDto() { Key = new Guid("268D4007-8853-455A-89F7-A28398843E5F"), Name = "Default Warehouse", CountryCode = "", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }
    }
}
