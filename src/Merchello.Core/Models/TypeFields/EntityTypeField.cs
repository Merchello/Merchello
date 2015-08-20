namespace Merchello.Core.Models.TypeFields
{
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Configuration.Outline;

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

        /// <summary>
        /// Gets the custom type fields.
        /// </summary>
        public override IEnumerable<ITypeField> CustomTypeFields
        {
            get
            {
                return Entities.GetTypeFields().Select(GetTypeField);
            }
        }

        /// <summary>
        /// Gets the campaign offer.
        /// </summary>
        public ITypeField CampaignOffer
        {
            get
            {
                return GetTypeField(EntityType.CampaignOffer);
            }
        }

        /// <summary>
        /// Gets the customer entity type field
        /// </summary>
        public ITypeField Customer 
        {
            get { return GetTypeField(EntityType.Customer); }
        }

        /// <summary>
        /// Gets the entity collection.
        /// </summary>
        public ITypeField EntityCollection
        {
            get
            {
                return GetTypeField(EntityType.EntityCollection);
            }
        }

        /// <summary>
        /// Gets the GatewayProvider entity type field
        /// </summary>
        public ITypeField GatewayProvider 
        {
            get { return GetTypeField(EntityType.GatewayProvider); }
        }

        /// <summary>
        /// Gets the Invoice entity type field
        /// </summary>
        public ITypeField Invoice
        {
            get { return GetTypeField(EntityType.Invoice); }
        }

        /// <summary>
        /// Gets the ItemCache entity type field
        /// </summary>
        public ITypeField ItemCache
        {
            get { return GetTypeField(EntityType.ItemCache); }
        }

        /// <summary>
        /// Gets the order entity type field
        /// </summary>
        public ITypeField Order
        {
            get { return GetTypeField(EntityType.Order); }
        }

        /// <summary>
        /// Gets the payment entity type field
        /// </summary>
        public ITypeField Payment
        {
            get { return GetTypeField(EntityType.Payment); }
        }

        /// <summary>
        /// Gets the product entity type field
        /// </summary>
        public ITypeField Product 
        {
            get { return GetTypeField(EntityType.Product); }
        }

        /// <summary>
        /// Gets the shipment entity type field
        /// </summary>
        public ITypeField Shipment
        {
            get { return GetTypeField(EntityType.Shipment); }
        }

        /// <summary>
        /// Gets the Warehouse entity type field
        /// </summary>
        public ITypeField Warehouse
        {
            get { return GetTypeField(EntityType.Warehouse); }
        }

        /// <summary>
        /// Gets the warehouse catalog entity type field
        /// </summary>
        public ITypeField WarehouseCatalog
        {
            get { return GetTypeField(EntityType.WarehouseCatalog); }
        }

        /// <summary>
        /// Gets the entities.
        /// </summary>
        internal static TypeFieldCollection Entities
        {
            get { return Fields.Entities; }
        }

        /// <summary>
        /// The build cache.
        /// </summary>
        internal override void BuildCache()
        {
            AddUpdateCache(EntityType.CampaignOffer, new TypeField("CampaignActivity", "CampaignActivity", Constants.TypeFieldKeys.Entity.CampaignOfferKey));
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
            AddUpdateCache(EntityType.EntityCollection, new TypeField("EntityCollection", "EntityCollection", Constants.TypeFieldKeys.Entity.EntityCollectionKey));
            AddUpdateCache(EntityType.Custom, NotFound);
        }

        /// <summary>
        /// Returns a custom address or NullTypeField TypeKey (GUID)
        /// </summary>
        /// <param name="alias">The alias of the custom entities</param>
        /// <returns>An object of <see cref="ITypeField"/></returns>
        protected override ITypeField GetCustom(string alias)
        {
            return GetTypeField(Entities[alias]);
        }

    }
}