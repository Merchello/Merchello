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
        public virtual DbSet<MerchInvoiceIndex> MerchInvoiceIndex { get; set; }
        public virtual DbSet<MerchInvoiceItem> MerchInvoiceItem { get; set; }
        public virtual DbSet<MerchInvoiceStatus> MerchInvoiceStatus { get; set; }
        public virtual DbSet<MerchItemCache> MerchItemCache { get; set; }
        public virtual DbSet<MerchItemCacheItem> MerchItemCacheItem { get; set; }
        public virtual DbSet<MerchNote> MerchNote { get; set; }
        public virtual DbSet<MerchNotificationMessage> MerchNotificationMessage { get; set; }
        public virtual DbSet<MerchNotificationMethod> MerchNotificationMethod { get; set; }
        public virtual DbSet<MerchOfferRedeemed> MerchOfferRedeemed { get; set; }
        public virtual DbSet<MerchOfferSettings> MerchOfferSettings { get; set; }
        public virtual DbSet<MerchOrder> MerchOrder { get; set; }
        public virtual DbSet<MerchOrderIndex> MerchOrderIndex { get; set; }
        public virtual DbSet<MerchOrderItem> MerchOrderItem { get; set; }
        public virtual DbSet<MerchOrderStatus> MerchOrderStatus { get; set; }
        public virtual DbSet<MerchPayment> MerchPayment { get; set; }
        public virtual DbSet<MerchPaymentMethod> MerchPaymentMethod { get; set; }
        public virtual DbSet<MerchProduct> MerchProduct { get; set; }
        public virtual DbSet<MerchProduct2EntityCollection> MerchProduct2EntityCollection { get; set; }
        public virtual DbSet<MerchProduct2ProductOption> MerchProduct2ProductOption { get; set; }
        public virtual DbSet<MerchProductAttribute> MerchProductAttribute { get; set; }
        public virtual DbSet<MerchProductOption> MerchProductOption { get; set; }
        public virtual DbSet<MerchProductOptionAttributeShare> MerchProductOptionAttributeShare { get; set; }
        public virtual DbSet<MerchProductVariant> MerchProductVariant { get; set; }
        public virtual DbSet<MerchProductVariant2ProductAttribute> MerchProductVariant2ProductAttribute { get; set; }
        public virtual DbSet<MerchProductVariantDetachedContent> MerchProductVariantDetachedContent { get; set; }
        public virtual DbSet<MerchProductVariantIndex> MerchProductVariantIndex { get; set; }
        public virtual DbSet<MerchShipCountry> MerchShipCountry { get; set; }
        public virtual DbSet<MerchShipMethod> MerchShipMethod { get; set; }
        public virtual DbSet<MerchShipRateTier> MerchShipRateTier { get; set; }
        public virtual DbSet<MerchShipment> MerchShipment { get; set; }
        public virtual DbSet<MerchShipmentStatus> MerchShipmentStatus { get; set; }
        public virtual DbSet<MerchStoreSetting> MerchStoreSetting { get; set; }
        public virtual DbSet<MerchTaxMethod> MerchTaxMethod { get; set; }
        public virtual DbSet<MerchTypeField> MerchTypeField { get; set; }
        public virtual DbSet<MerchWarehouse> MerchWarehouse { get; set; }
        public virtual DbSet<MerchWarehouseCatalog> MerchWarehouseCatalog { get; set; }

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