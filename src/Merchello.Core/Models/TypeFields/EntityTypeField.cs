namespace Merchello.Core.Models.TypeFields
{
    /// <summary>
    /// Type fields for Merchello entities
    /// </summary>
    public sealed class EntityTypeField : TypeFieldMapper<EntityType>, IEntityTypeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityTypeField"/> class.
        /// </summary>
        internal EntityTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        /// <inheritdoc/>
        public ITypeField Customer 
        {
            get
            {
                return GetTypeField(EntityType.Customer);
            }
        }

        /// <inheritdoc/>
        public ITypeField EntityCollection
        {
            get
            {
                return GetTypeField(EntityType.EntityCollection);
            }
        }

        /// <inheritdoc/>
        public ITypeField GatewayProvider 
        {
            get
            {
                return GetTypeField(EntityType.GatewayProvider);
            }
        }

        /// <inheritdoc/>
        public ITypeField Invoice
        {
            get
            {
                return GetTypeField(EntityType.Invoice);
            }
        }

        /// <inheritdoc/>
        public ITypeField ItemCache
        {
            get
            {
                return GetTypeField(EntityType.ItemCache);
            }
        }

        /// <inheritdoc/>
        public ITypeField Order
        {
            get
            {
                return GetTypeField(EntityType.Order);
            }
        }

        /// <inheritdoc/>
        public ITypeField Payment
        {
            get
            {
                return GetTypeField(EntityType.Payment);
            }
        }

        /// <inheritdoc/>
        public ITypeField Product 
        {
            get
            {
                return GetTypeField(EntityType.Product);
            }
        }

        /// <inheritdoc/>
        public ITypeField ProductOption
        {
            get
            {
                return GetTypeField(EntityType.ProductOption);
            }
        }

        /// <inheritdoc/>
        public ITypeField Shipment
        {
            get
            {
                return GetTypeField(EntityType.Shipment);
            }
        }

        /// <inheritdoc/>
        public ITypeField Warehouse
        {
            get
            {
                return GetTypeField(EntityType.Warehouse);
            }
        }

        /// <inheritdoc/>
        public ITypeField WarehouseCatalog
        {
            get { return GetTypeField(EntityType.WarehouseCatalog); }
        }


        /// <inheritdoc/>
        internal override void BuildCache()
        {
            AddUpdateCache(EntityType.Customer, new TypeField("Customer", "Customer", Constants.TypeFieldKeys.Entity.CustomerKey));
            AddUpdateCache(EntityType.GatewayProvider, new TypeField("GatewayProvider", "GatewayProvider", Constants.TypeFieldKeys.Entity.GatewayProviderKey));
            AddUpdateCache(EntityType.Invoice, new TypeField("Invoice", "Invoice", Constants.TypeFieldKeys.Entity.InvoiceKey));
            AddUpdateCache(EntityType.ItemCache, new TypeField("ItemCache", "ItemCache", Constants.TypeFieldKeys.Entity.ItemCacheKey));
            AddUpdateCache(EntityType.Order, new TypeField("Order", "Order", Constants.TypeFieldKeys.Entity.OrderKey));
            AddUpdateCache(EntityType.Payment, new TypeField("Payment", "Payment", Constants.TypeFieldKeys.Entity.PaymentKey));
            AddUpdateCache(EntityType.Product, new TypeField("Product", "Product", Constants.TypeFieldKeys.Entity.ProductKey));
            AddUpdateCache(EntityType.ProductOption, new TypeField("ProductOption", "ProductOption", Constants.TypeFieldKeys.Entity.ProductOptionKey));
            AddUpdateCache(EntityType.Shipment, new TypeField("Shipment", "Shipment", Constants.TypeFieldKeys.Entity.ShipmentKey));
            AddUpdateCache(EntityType.Warehouse, new TypeField("Warehouse", "Warehouse", Constants.TypeFieldKeys.Entity.WarehouseKey));
            AddUpdateCache(EntityType.WarehouseCatalog, new TypeField("WarehouseCatalog", "WarehouseCatalog", Constants.TypeFieldKeys.Entity.WarehouseCatalogKey));
            AddUpdateCache(EntityType.EntityCollection, new TypeField("EntityCollection", "EntityCollection", Constants.TypeFieldKeys.Entity.EntityCollectionKey));
        }
    }
}