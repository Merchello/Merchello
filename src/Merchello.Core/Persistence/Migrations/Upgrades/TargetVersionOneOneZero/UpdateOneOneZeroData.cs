using System;
using Merchello.Core.Events;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence.Migrations.Upgrades.TargetVersionOneOneZero
{
    /// <summary>
    /// Represents the initial data creation by running Insert for the version 1.1.0 default data.
    /// </summary>
    internal class UpdateOneOneZeroData
    {
        private readonly Database _database;

        public UpdateOneOneZeroData(Database database)
        {
            _database = database;
        }


        public void InitializeVersionData(string tableName)
        {
            if (tableName.Equals("merchTypeField")) CreateDbTypeFieldData();

            if (tableName.Equals("merchNotificationTrigger")) CreateNotificationTriggerData();
        }

        private void CreateNotificationTriggerData()
        {
            // invoice status
            
            _database.Insert("merchNotificationTrigger", "Key", new NotificationTriggerDto() { Key = Constants.NotificationTriggerKeys.InvoiceService.StatusChanged.ToPaid, Name = "Invoice Status Changed To Paid", Binding = NotificationTriggerService.GetBindingValue(typeof(InvoiceService), typeof(StatusChangeEventArgs<>)), EntityKey = Constants.DefaultKeys.InvoiceStatus.Paid, UpdateDate = DateTime.Now, CreateDate = DateTime.Now});
            _database.Insert("merchNotificationTrigger", "Key", new NotificationTriggerDto() { Key = Constants.NotificationTriggerKeys.InvoiceService.StatusChanged.ToPartial, Name = "Invoice Status Changed To Partial Paid", Binding = NotificationTriggerService.GetBindingValue(typeof(InvoiceService), typeof(StatusChangeEventArgs<>)), EntityKey = Constants.DefaultKeys.InvoiceStatus.Partial, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchNotificationTrigger", "Key", new NotificationTriggerDto() { Key = Constants.NotificationTriggerKeys.InvoiceService.StatusChanged.ToCancelled, Name = "Invoice Status Changed To Cancelled", Binding = NotificationTriggerService.GetBindingValue(typeof(InvoiceService), typeof(StatusChangeEventArgs<>)), EntityKey = Constants.DefaultKeys.InvoiceStatus.Cancelled, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchNotificationTrigger", "Key", new NotificationTriggerDto() { Key = Constants.NotificationTriggerKeys.OrderService.StatusChanged.ToFulfilled, Name = "Order Status Changed To Fulfilled", Binding = NotificationTriggerService.GetBindingValue(typeof(OrderService), typeof(StatusChangeEventArgs<>)), EntityKey = Constants.DefaultKeys.OrderStatus.Fulfilled, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchNotificationTrigger", "Key", new NotificationTriggerDto() { Key = Constants.NotificationTriggerKeys.OrderService.StatusChanged.ToBackOrder, Name = "Order Status Changed To Back Order", Binding = NotificationTriggerService.GetBindingValue(typeof(OrderService), typeof(StatusChangeEventArgs<>)), EntityKey = Constants.DefaultKeys.OrderStatus.BackOrder, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
            _database.Insert("merchNotificationTrigger", "Key", new NotificationTriggerDto() { Key = Constants.NotificationTriggerKeys.OrderService.StatusChanged.ToCancelled, Name = "Order Status Changed To Cancelled", Binding = NotificationTriggerService.GetBindingValue(typeof(OrderService), typeof(StatusChangeEventArgs<>)), EntityKey = Constants.DefaultKeys.OrderStatus.Cancelled, UpdateDate = DateTime.Now, CreateDate = DateTime.Now });
        }

        private void CreateDbTypeFieldData()
        {
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
    }
}