namespace Merchello.Core.Data.Contexts
{
    using Merchello.Core.Data.Mappings;
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchelloDbContext : DbContext
    {
        private readonly IDbEntityRegister entityRegister;

        public MerchelloDbContext(DbContextOptions options, IDbEntityRegister entityRegister)
            : base(options)
        {
            this.entityRegister = entityRegister;
        }

        #region  DbSets

        public virtual DbSet<AnonymousCustomerDto> MerchAnonymousCustomer { get; set; }
        public virtual DbSet<AppliedPaymentDto> MerchAppliedPayment { get; set; }
        public virtual DbSet<AuditLogDto> MerchAuditLog { get; set; }
        public virtual DbSet<CatalogInventoryDto> MerchCatalogInventory { get; set; }
        public virtual DbSet<CustomerDto> MerchCustomer { get; set; }
        public virtual DbSet<Customer2EntityCollectionDto> MerchCustomer2EntityCollection { get; set; }
        public virtual DbSet<CustomerAddressDto> MerchCustomerAddress { get; set; }
        public virtual DbSet<CustomerIndexDto> MerchCustomerIndex { get; set; }
        public virtual DbSet<DetachedContentTypeDto> MerchDetachedContentType { get; set; }
        public virtual DbSet<DigitalMediaDto> MerchDigitalMedia { get; set; }
        public virtual DbSet<EntityCollectionDto> MerchEntityCollection { get; set; }
        public virtual DbSet<GatewayProviderSettingsDto> MerchGatewayProviderSettings { get; set; }
        public virtual DbSet<InvoiceDto> MerchInvoice { get; set; }
        public virtual DbSet<Invoice2EntityCollectionDto> MerchInvoice2EntityCollection { get; set; }
        public virtual DbSet<InvoiceIndexDto> MerchInvoiceIndex { get; set; }
        public virtual DbSet<InvoiceItemDto> MerchInvoiceItem { get; set; }
        public virtual DbSet<InvoiceStatusDto> MerchInvoiceStatus { get; set; }
        public virtual DbSet<ItemCacheDto> MerchItemCache { get; set; }
        public virtual DbSet<ItemCacheItemDto> MerchItemCacheItem { get; set; }
        public virtual DbSet<NoteDto> MerchNote { get; set; }
        public virtual DbSet<NotificationMessageDto> MerchNotificationMessage { get; set; }
        public virtual DbSet<NotificationMethodDto> MerchNotificationMethod { get; set; }
        public virtual DbSet<OfferRedeemedDto> MerchOfferRedeemed { get; set; }
        public virtual DbSet<OfferSettingsDto> MerchOfferSettings { get; set; }
        public virtual DbSet<OrderDto> MerchOrder { get; set; }
        public virtual DbSet<OrderIndexDto> MerchOrderIndex { get; set; }
        public virtual DbSet<OrderItemDto> MerchOrderItem { get; set; }
        public virtual DbSet<OrderStatusDto> MerchOrderStatus { get; set; }
        public virtual DbSet<PaymentDto> MerchPayment { get; set; }
        public virtual DbSet<PaymentMethodDto> MerchPaymentMethod { get; set; }
        public virtual DbSet<ProductDto> MerchProduct { get; set; }
        public virtual DbSet<Product2EntityCollectionDto> MerchProduct2EntityCollection { get; set; }
        public virtual DbSet<Product2ProductOptionDto> MerchProduct2ProductOption { get; set; }
        public virtual DbSet<ProductAttributeDto> MerchProductAttribute { get; set; }
        public virtual DbSet<ProductOptionDto> MerchProductOption { get; set; }
        public virtual DbSet<ProductOptionAttributeShareDto> MerchProductOptionAttributeShare { get; set; }
        public virtual DbSet<ProductVariantDto> MerchProductVariant { get; set; }
        public virtual DbSet<ProductVariant2ProductAttributeDto> MerchProductVariant2ProductAttribute { get; set; }
        public virtual DbSet<ProductVariantDetachedContentDto> MerchProductVariantDetachedContent { get; set; }
        public virtual DbSet<ProductVariantIndexDto> MerchProductVariantIndex { get; set; }
        public virtual DbSet<ShipCountryDto> MerchShipCountry { get; set; }
        public virtual DbSet<ShipMethodDto> MerchShipMethod { get; set; }
        public virtual DbSet<ShipRateTierDto> MerchShipRateTier { get; set; }
        public virtual DbSet<ShipmentDto> MerchShipment { get; set; }
        public virtual DbSet<ShipmentStatusDto> MerchShipmentStatus { get; set; }
        public virtual DbSet<StoreSettingDto> MerchStoreSetting { get; set; }
        public virtual DbSet<TaxMethodDto> MerchTaxMethod { get; set; }
        public virtual DbSet<TypeFieldDto> MerchTypeField { get; set; }
        public virtual DbSet<WarehouseDto> MerchWarehouse { get; set; }
        public virtual DbSet<WarehouseCatalogDto> MerchWarehouseCatalog { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            //optionsBuilder.UseSqlServer(@"Server=.\MSSQLSERVER2016;database=Merchello;Trusted_Connection=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            foreach (var entityMap in this.entityRegister.GetInstantiations())
            {
                entityMap.Configure(modelBuilder);
            }           
        }
    }
}