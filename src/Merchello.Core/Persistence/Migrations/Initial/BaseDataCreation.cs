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

        private static readonly Guid WarehouseKey = new Guid("268D4007-8853-455A-89F7-A28398843E5F");
        private static readonly Guid WarehouseCatalogKey = new Guid("B25C2B00-578E-49B9-BEA2-BF3712053C63");

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

            if(tableName.Equals("merchOrderStatus")) CreateOrderStatusData();            
            
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
            

            var litf = new LineItemTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = litf.Product.TypeKey, Alias = litf.Product.Alias, Name = litf.Product.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = litf.Shipping.TypeKey, Alias = litf.Shipping.Alias, Name = litf.Shipping.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = litf.Tax.TypeKey, Alias = litf.Tax.Alias, Name = litf.Tax.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = litf.Discount.TypeKey, Alias = litf.Discount.Alias, Name = litf.Discount.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

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

            //// AppliedPaymentType
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
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("1F872A1A-F0DD-4C3E-80AB-99799A28606E"), Alias = "paid", Name = "Paid", Active = true, Reportable = true, SortOrder = 2, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("6606B0EA-15B6-44AA-8557-B2D9D049645C"), Alias = "partial", Name = "Partial", Active = true, Reportable = true, SortOrder = 3, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("53077EFD-6BF0-460D-9565-0E00567B5176"), Alias = "cancelled", Name = "Cancelled", Active = true, Reportable = true, SortOrder = 4, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = new Guid("75E1E5EB-33E8-4904-A8E5-4B64A37D6087"), Alias = "fraud", Name = "Fraud", Active = true, Reportable = true, SortOrder = 5, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        private void CreateOrderStatusData()
        {
            _database.Insert("merchOrderStatus", "Key", new InvoiceStatusDto() { Key = new Guid("C54D47E6-D1C9-40D5-9BAF-18C6ADFFE9D0"), Alias = "notfulfilled", Name = "Not Fulfilled", Active = true, Reportable = true, SortOrder = 1, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchOrderStatus", "Key", new InvoiceStatusDto() { Key = new Guid("D5369B84-8CCA-4586-8FBA-F3020F5E06EC"), Alias = "fulfilled", Name = "Fulfilled", Active = true, Reportable = true, SortOrder = 2, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchOrderStatus", "Key", new InvoiceStatusDto() { Key = new Guid("C47D475F-A075-4635-BBB9-4B9C49AA8EBE"), Alias = "partial", Name = "Partial", Active = true, Reportable = true, SortOrder = 3, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchOrderStatus", "Key", new InvoiceStatusDto() { Key = new Guid("77DAF52E-C79C-4E1B-898C-5E977A9A6027"), Alias = "cancelled", Name = "Cancelled", Active = true, Reportable = true, SortOrder = 4, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });            
        }


        private void CreateWarehouseData()
        {
            _database.Insert("merchWarehouse", "Key", new WarehouseDto() { Key = WarehouseKey, Name = "Default Warehouse", CountryCode = "", IsDefault = true, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchWarehouseCatalog", "Key", new WarehouseCatalogDto() { Key = WarehouseCatalogKey, WarehouseKey = WarehouseKey, Name = "Default Catalog", Description = null, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

    }
}
