using Merchello.Core.Configuration.Outline;

namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Type fields for Merchello entities
    /// </summary>
    internal sealed class EntityTypeField : TypeFieldMapper<EntityType>, IEntityTypeField
    {

        internal EntityTypeField()
        {
            if (CachedTypeFields.IsEmpty) BuildCache();
        }

        internal override void BuildCache()
        {
            AddUpdateCache(EntityType.Customer, new TypeField("Customer", "Customer", Constants.TypeFieldKeys.Entity.CustomerKey));
            AddUpdateCache(EntityType.GatewayProvider, new TypeField("GatewayProvider", "GatewayProvider", Constants.TypeFieldKeys.Entity.GatewayProviderKey));
            AddUpdateCache(EntityType.Invoice, new TypeField("Invoice", "Invoice", Constants.TypeFieldKeys.Entity.InvoiceKey));
            AddUpdateCache(EntityType.ItemCache, new TypeField("ItemCache", "ItemCache", Constants.TypeFieldKeys.Entity.ItemCacheKey));
            AddUpdateCache(EntityType.Order, new TypeField("Order", "Order", Constants.TypeFieldKeys.Entity.OrderKey));
            AddUpdateCache(EntityType.Payment, new TypeField("Payment", "Payment", Constants.TypeFieldKeys.Entity.PaymentKey));
            AddUpdateCache(EntityType.Product, new TypeField("Product", "Product", Constants.TypeFieldKeys.Entity.ProductKey));
            AddUpdateCache(EntityType.Shipment, new TypeField("Shipment", "Shipment", Constants.TypeFieldKeys.Entity.ShipmentKey));
            AddUpdateCache(EntityType.Warehouse, new TypeField("Warehouse", "Warehouse", Constants.TypeFieldKeys.Entity.WarehouseKey));
            AddUpdateCache(EntityType.WarehouseCatalog, new TypeField("WarehouseCatalog", "WarehouseCatalog", Constants.TypeFieldKeys.Entity.WarehouseCatalogKey));
        }

        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return Entities.GetTypeFields().Select(GetTypeField);
            }
        }


        /// <summary>
        /// The customer entity type field
        /// </summary>
        public ITypeField Customer {
            get { return GetTypeField(EntityType.Customer); }
        }

        /// <summary>
        /// The GatewayProvider entity type field
        /// </summary>
        public ITypeField GatewayProvider {
            get { return GetTypeField(EntityType.GatewayProvider); }
        }

        /// <summary>
        /// The Invoice entity type field
        /// </summary>
        public ITypeField Invoice
        {
            get { return GetTypeField(EntityType.Invoice); }
        }

        /// <summary>
        /// The ItemCache entity type field
        /// </summary>
        public ITypeField ItemCache
        {
            get { return GetTypeField(EntityType.ItemCache); }
        }

        /// <summary>
        /// The order entity type field
        /// </summary>
        public ITypeField Order
        {
            get { return GetTypeField(EntityType.Order); }
        }

        /// <summary>
        /// The payment entity type field
        /// </summary>
        public ITypeField Payment
        {
            get { return GetTypeField(EntityType.Payment); }
        }

        /// <summary>
        /// The product entity type field
        /// </summary>
        public ITypeField Product {
            get { return GetTypeField(EntityType.Product); }
        }

        /// <summary>
        /// The shipment entity type field
        /// </summary>
        public ITypeField Shipment
        {
            get { return GetTypeField(EntityType.Shipment); }
        }

        /// <summary>
        /// The Warehouse entity type field
        /// </summary>
        public ITypeField Warehouse
        {
            get { return GetTypeField(EntityType.Warehouse); }
        }

        /// <summary>
        /// The warehouse catalog entity type field
        /// </summary>
        public ITypeField WarehouseCatalog
        {
            get { return GetTypeField(EntityType.WarehouseCatalog); }
        }

        /// <summary>
        /// Returns a custom address or NullTypeField TypeKey (Guid)
        /// </summary>
        /// <param name="alias">The alias of the custom entitie</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(Entities[alias]);
        }

        private static TypeFieldCollection Entities
        {
            get { return Fields.Entities; }
        }
    }
}