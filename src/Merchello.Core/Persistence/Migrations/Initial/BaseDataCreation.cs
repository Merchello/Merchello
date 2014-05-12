using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero;
using Newtonsoft.Json;
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

            if(tableName.Equals("merchOrderStatus")) CreateOrderStatusData();   
         
            if(tableName.EndsWith("merchGatewayProvider")) CreateGatewayProviderData();

            if(tableName.EndsWith("merchStoreSetting")) CreateStoreSettingData();            
            
        }

        private void CreateDbTypeFieldData()
        {
            // address
            var address = new AddressTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = address.Shipping.TypeKey, Alias = address.Shipping.Alias, Name = address.Shipping.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now});
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = address.Billing.TypeKey, Alias = address.Billing.Alias, Name = address.Billing.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // ItemCacheTypeField
            var itemcCache = new ItemCacheTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itemcCache.Basket.TypeKey, Alias = itemcCache.Basket.Alias, Name = itemcCache.Basket.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itemcCache.Wishlist.TypeKey, Alias = itemcCache.Wishlist.Alias, Name = itemcCache.Wishlist.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = itemcCache.Checkout.TypeKey, Alias = itemcCache.Checkout.Alias, Name = itemcCache.Checkout.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            
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

            //// AppliedPaymentType
            var apf = new AppliedPaymentTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Debit.TypeKey, Alias = apf.Debit.Alias, Name = apf.Debit.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Credit.TypeKey, Alias = apf.Credit.Alias, Name = apf.Credit.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Void.TypeKey, Alias = apf.Void.Alias, Name = apf.Void.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Denied.TypeKey, Alias = apf.Denied.Alias, Name = apf.Denied.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = apf.Refund.TypeKey, Alias = apf.Refund.Alias, Name = apf.Refund.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // GatewayProviderType
            var gwp = new GatewayProviderTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Payment.TypeKey, Alias = gwp.Payment.Alias, Name = gwp.Payment.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Shipping.TypeKey, Alias = gwp.Shipping.Alias, Name = gwp.Shipping.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Taxation.TypeKey, Alias = gwp.Taxation.Alias, Name = gwp.Taxation.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = gwp.Notification.TypeKey, Alias = gwp.Notification.Alias, Name = gwp.Notification.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });

            // EntityTypeField
            var entity = new EntityTypeField();
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Customer.TypeKey, Alias = entity.Customer.Alias, Name = entity.Customer.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.GatewayProvider.TypeKey, Alias = entity.GatewayProvider.Alias, Name = entity.GatewayProvider.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Invoice.TypeKey, Alias = entity.Invoice.Alias, Name = entity.Invoice.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.ItemCache.TypeKey, Alias = entity.ItemCache.Alias, Name = entity.ItemCache.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Order.TypeKey, Alias = entity.Order.Alias, Name = entity.Order.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Payment.TypeKey, Alias = entity.Payment.Alias, Name = entity.Payment.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Product.TypeKey, Alias = entity.Product.Alias, Name = entity.Product.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Shipment.TypeKey, Alias = entity.Shipment.Alias, Name = entity.Shipment.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.Warehouse.TypeKey, Alias = entity.Warehouse.Alias, Name = entity.Warehouse.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchTypeField", "Key", new TypeFieldDto() { Key = entity.WarehouseCatalog.TypeKey, Alias = entity.WarehouseCatalog.Alias, Name = entity.WarehouseCatalog.Name, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
        }

        private void CreateInvoiceStatusData()
        {
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = Constants.DefaultKeys.InvoiceStatus.Unpaid, Alias = "unpaid", Name = "Unpaid", Active = true, Reportable = true, SortOrder = 1, CreateDate = DateTime.Now, UpdateDate = DateTime.Now});
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = Constants.DefaultKeys.InvoiceStatus.Paid, Alias = "paid", Name = "Paid", Active = true, Reportable = true, SortOrder = 2, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = Constants.DefaultKeys.InvoiceStatus.Partial, Alias = "partial", Name = "Partial", Active = true, Reportable = true, SortOrder = 3, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = Constants.DefaultKeys.InvoiceStatus.Cancelled, Alias = "cancelled", Name = "Cancelled", Active = true, Reportable = true, SortOrder = 4, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchInvoiceStatus", "Key", new InvoiceStatusDto() { Key = Constants.DefaultKeys.InvoiceStatus.Fraud, Alias = "fraud", Name = "Fraud", Active = true, Reportable = true, SortOrder = 5, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        private void CreateOrderStatusData()
        {
            _database.Insert("merchOrderStatus", "Key", new OrderStatusDto() { Key = Constants.DefaultKeys.OrderStatus.NotFulfilled, Alias = "notfulfilled", Name = "Not Fulfilled", Active = true, Reportable = true, SortOrder = 1, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchOrderStatus", "Key", new OrderStatusDto() { Key = Constants.DefaultKeys.OrderStatus.Fulfilled, Alias = "fulfilled", Name = "Fulfilled", Active = true, Reportable = true, SortOrder = 2, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchOrderStatus", "Key", new OrderStatusDto() { Key = Constants.DefaultKeys.OrderStatus.BackOrder, Alias = "backOrder", Name = "BackOrder", Active = true, Reportable = true, SortOrder = 3, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchOrderStatus", "Key", new OrderStatusDto() { Key = Constants.DefaultKeys.OrderStatus.Cancelled, Alias = "cancelled", Name = "Cancelled", Active = true, Reportable = true, SortOrder = 4, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });            
        }


        private void CreateWarehouseData()
        {
            _database.Insert("merchWarehouse", "Key", new WarehouseDto() { Key = Constants.DefaultKeys.Warehouse.DefaultWarehouseKey, Name = "Default Warehouse", CountryCode = "", IsDefault = true, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchWarehouseCatalog", "Key", new WarehouseCatalogDto() { Key = Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey, WarehouseKey = Constants.DefaultKeys.Warehouse.DefaultWarehouseKey, Name = "Default Catalog", Description = null, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

        private void CreateGatewayProviderData()
        {
            var extended = new ExtendedDataCollection();

            // TODO - move this to a package action
            _database.Insert("merchGatewayProvider", "Key", new GatewayProviderDto() { Key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey, Name = "Fixed Rate Shipping Provider", ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Shipping).TypeKey, ExtendedData = new ExtendedDataCollection().SerializeToXml(), EncryptExtendedData = false, TypeFullName = "Merchello.Core.Gateways.Shipping.FixedRate.FixedRateShippingGatewayProvider, Merchello.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });

            // add the everywhere else shipcountry
            _database.Insert("merchShipCountry", "Key",
                             new ShipCountryDto()
                                 {
                                     Key = Guid.NewGuid(),
                                     CatalogKey = Constants.DefaultKeys.Warehouse.DefaultWarehouseCatalogKey,
                                     CountryCode = Constants.CountryCodes.EverywhereElse,
                                     Name = "Everywhere Else",
                                     CreateDate = DateTime.Now,
                                     UpdateDate = DateTime.Now
                                 });

            // TODO - move this to a package action
            _database.Insert("merchGatewayProvider", "Key", new GatewayProviderDto() { Key = Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey, Name = "Fixed Rate Tax Provider", ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Taxation).TypeKey, ExtendedData = new ExtendedDataCollection().SerializeToXml(), EncryptExtendedData = false, TypeFullName = "Merchello.Core.Gateways.Taxation.FixedRate.FixedRateTaxationGatewayProvider, Merchello.Core, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });

            var taxProvinces = new ProvinceCollection<ITaxProvince>();
            var provinceData = JsonConvert.SerializeObject(taxProvinces);

            _database.Insert("merchTaxMethod", "Key",
                             new TaxMethodDto()
                                 {
                                     Key = Guid.NewGuid(),
                                     CountryCode = Constants.CountryCodes.EverywhereElse,
                                     Name = "Everywhere Else",
                                     PercentageTaxRate = 0,
                                     ProviderKey = Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey,
                                     ProvinceData = provinceData,
                                     UpdateDate = DateTime.Now,
                                     CreateDate = DateTime.Now
                                 });

            // TODO - move this to a package action
            _database.Insert("merchGatewayProvider", "Key", new GatewayProviderDto() { Key = Constants.ProviderKeys.Payment.CashPaymentProviderKey, Name = "Cash Payment Provider", ProviderTfKey = EnumTypeFieldConverter.GatewayProvider.GetTypeField(GatewayProviderType.Payment).TypeKey, ExtendedData = new ExtendedDataCollection().SerializeToXml(), EncryptExtendedData = false, TypeFullName = "Merchello.Core.Gateways.Payment.Cash.CashPaymentGatewayProvider, Merchello.Core, Version=1.0.1.0, Culture=neutral, PublicKeyToken=null", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });

            _database.Insert("merchPaymentMethod", "Key",
                new PaymentMethodDto()
                {
                    Key = Guid.NewGuid(),
                    Name = "Cash",
                    PaymentCode = "Cash",
                    Description = "Cash Payment",
                    ProviderKey = Constants.ProviderKeys.Payment.CashPaymentProviderKey,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now
                });
        }

        private void CreateStoreSettingData()
        {
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.CurrencyCodeKey, Name = "currencyCode", Value = "USD", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now});
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.NextOrderNumberKey, Name = "nextOrderNumber", Value = "1", TypeName = "System.Int32", CreateDate = DateTime.Now, UpdateDate = DateTime.Now});
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.NextInvoiceNumberKey, Name = "nextInvoiceNumber", Value = "1", TypeName = "System.Int32", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.DateFormatKey, Name = "dateFormat", Value = "dd-mm-yyyy", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.TimeFormatKey, Name = "timeFormat", Value = "am-pm", TypeName = "System.String", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.GlobalShippableKey, Name = "globalShippable", Value = "false", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.GlobalTaxableKey, Name = "globalTaxable", Value = "false", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.GlobalTrackInventoryKey, Name = "globalTrackInventory", Value = "false", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
            _database.Insert("merchStoreSetting", "Key", new StoreSettingDto() { Key = Constants.StoreSettingKeys.GlobalShippingIsTaxableKey, Name = "globalShippingIsTaxable", Value = "false", TypeName = "System.Boolean", CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
        }

    }
}
