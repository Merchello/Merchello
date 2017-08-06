namespace Merchello.Core.Data.Contexts
{
    using Merchello.Core.Data.Mappings;
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal partial class MerchelloDbContext : DbContext
    {
        private readonly IDbEntityRegister entityRegister;

        public MerchelloDbContext(IDbEntityRegister entityRegister)
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
           
            modelBuilder.Entity<MerchInvoiceIndex>(entity =>
            {
                entity.ToTable("merchInvoiceIndex");

                entity.HasIndex(e => e.InvoiceKey)
                    .HasName("IX_merchInvoiceIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                    .WithOne(p => p.MerchInvoiceIndex)
                    .HasForeignKey<MerchInvoiceIndex>(d => d.InvoiceKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchInvoiceIndex_merchInvoice");
            });

            modelBuilder.Entity<MerchInvoiceItem>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchInvoiceItem");

                entity.ToTable("merchInvoiceItem");

                entity.HasIndex(e => e.Sku)
                    .HasName("IX_merchInvoiceItemSku");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Exported).HasColumnName("exported");

                entity.Property(e => e.ExtendedData)
                    .HasColumnName("extendedData")
                    .HasColumnType("ntext");

                entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                entity.Property(e => e.LineItemTfKey).HasColumnName("lineItemTfKey");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                    .WithMany(p => p.MerchInvoiceItem)
                    .HasForeignKey(d => d.InvoiceKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchInvoiceItem_merchInvoice");
            });

            modelBuilder.Entity<MerchInvoiceStatus>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchInvoiceStatus");

                entity.ToTable("merchInvoiceStatus");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasColumnName("alias")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Reportable).HasColumnName("reportable");

                entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchItemCache>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchItemCache");

                entity.ToTable("merchItemCache");

                entity.HasIndex(e => e.EntityKey)
                    .HasName("IX_merchItemCacheEntityKey");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.EntityKey).HasColumnName("entityKey");

                entity.Property(e => e.ItemCacheTfKey).HasColumnName("itemCacheTfKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.VersionKey)
                    .HasColumnName("versionKey")
                    .HasDefaultValueSql("'newid()'");
            });

            modelBuilder.Entity<MerchItemCacheItem>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchItemCacheItem");

                entity.ToTable("merchItemCacheItem");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Exported).HasColumnName("exported");

                entity.Property(e => e.ExtendedData)
                    .HasColumnName("extendedData")
                    .HasColumnType("ntext");

                entity.Property(e => e.ItemCacheKey).HasColumnName("itemCacheKey");

                entity.Property(e => e.LineItemTfKey).HasColumnName("lineItemTfKey");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.ItemCacheKeyNavigation)
                    .WithMany(p => p.MerchItemCacheItem)
                    .HasForeignKey(d => d.ItemCacheKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchItemCacheItem_merchItemCache");
            });

            modelBuilder.Entity<MerchNote>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchNote");

                entity.ToTable("merchNote");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Author)
                    .HasColumnName("author")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.EntityKey).HasColumnName("entityKey");

                entity.Property(e => e.EntityTfKey).HasColumnName("entityTfKey");

                entity.Property(e => e.InternalOnly).HasColumnName("internalOnly");

                entity.Property(e => e.Message)
                    .HasColumnName("message")
                    .HasColumnType("ntext");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchNotificationMessage>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchNotificationMessage");

                entity.ToTable("merchNotificationMessage");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.BodyText)
                    .HasColumnName("bodyText")
                    .HasColumnType("ntext");

                entity.Property(e => e.BodyTextIsFilePath).HasColumnName("bodyTextIsFilePath");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.Disabled).HasColumnName("disabled");

                entity.Property(e => e.FromAddress)
                    .HasColumnName("fromAddress")
                    .HasMaxLength(255);

                entity.Property(e => e.MaxLength).HasColumnName("maxLength");

                entity.Property(e => e.MethodKey).HasColumnName("methodKey");

                entity.Property(e => e.MonitorKey).HasColumnName("monitorKey");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Recipients)
                    .IsRequired()
                    .HasColumnName("recipients")
                    .HasMaxLength(255);

                entity.Property(e => e.ReplyTo)
                    .HasColumnName("replyTo")
                    .HasMaxLength(255);

                entity.Property(e => e.SendToCustomer).HasColumnName("sendToCustomer");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.MethodKeyNavigation)
                    .WithMany(p => p.MerchNotificationMessage)
                    .HasForeignKey(d => d.MethodKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchNotificationMessage_merchNotificationMethod");
            });

            modelBuilder.Entity<MerchNotificationMethod>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchNotificationMethod");

                entity.ToTable("merchNotificationMethod");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                entity.Property(e => e.ServiceCode)
                    .IsRequired()
                    .HasColumnName("serviceCode")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.ProviderKeyNavigation)
                    .WithMany(p => p.MerchNotificationMethod)
                    .HasForeignKey(d => d.ProviderKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchNotificationMethod_merchGatewayProvider");
            });

            modelBuilder.Entity<MerchOfferRedeemed>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchOfferRedeemed");

                entity.ToTable("merchOfferRedeemed");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                entity.Property(e => e.ExtendedData)
                    .HasColumnName("extendedData")
                    .HasColumnType("ntext");

                entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                entity.Property(e => e.OfferCode)
                    .IsRequired()
                    .HasColumnName("offerCode")
                    .HasMaxLength(255);

                entity.Property(e => e.OfferProviderKey).HasColumnName("offerProviderKey");

                entity.Property(e => e.OfferSettingsKey).HasColumnName("offerSettingsKey");

                entity.Property(e => e.RedeemedDate)
                    .HasColumnName("redeemedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                    .WithMany(p => p.MerchOfferRedeemed)
                    .HasForeignKey(d => d.InvoiceKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchOfferRedeemed_merchInvoice");

                entity.HasOne(d => d.OfferSettingsKeyNavigation)
                    .WithMany(p => p.MerchOfferRedeemed)
                    .HasForeignKey(d => d.OfferSettingsKey)
                    .HasConstraintName("FK_merchOfferRedeemed_merchOfferSettings");
            });

            modelBuilder.Entity<MerchOfferSettings>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchOfferSettings");

                entity.ToTable("merchOfferSettings");

                entity.HasIndex(e => e.OfferCode)
                    .HasName("IX_merchOfferSettingsOfferCode")
                    .IsUnique();

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.ConfigurationData)
                    .HasColumnName("configurationData")
                    .HasColumnType("ntext");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.OfferCode)
                    .IsRequired()
                    .HasColumnName("offerCode")
                    .HasMaxLength(255);

                entity.Property(e => e.OfferEndsDate)
                    .HasColumnName("offerEndsDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.OfferProviderKey).HasColumnName("offerProviderKey");

                entity.Property(e => e.OfferStartsDate)
                    .HasColumnName("offerStartsDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchOrder>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchOrder");

                entity.ToTable("merchOrder");

                entity.HasIndex(e => e.OrderDate)
                    .HasName("IX_merchOrderDate");

                entity.HasIndex(e => e.OrderNumber)
                    .HasName("IX_merchOrderNumber")
                    .IsUnique();

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Exported).HasColumnName("exported");

                entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                entity.Property(e => e.OrderDate)
                    .HasColumnName("orderDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.OrderNumber).HasColumnName("orderNumber");

                entity.Property(e => e.OrderNumberPrefix)
                    .HasColumnName("orderNumberPrefix")
                    .HasMaxLength(255);

                entity.Property(e => e.OrderStatusKey).HasColumnName("orderStatusKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.VersionKey)
                    .HasColumnName("versionKey")
                    .HasDefaultValueSql("'newid()'");

                entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                    .WithMany(p => p.MerchOrder)
                    .HasForeignKey(d => d.InvoiceKey)
                    .HasConstraintName("FK_merchOrder_merchInvoice");

                entity.HasOne(d => d.OrderStatusKeyNavigation)
                    .WithMany(p => p.MerchOrder)
                    .HasForeignKey(d => d.OrderStatusKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchOrder_merchOrderStatus");
            });

            modelBuilder.Entity<MerchOrderIndex>(entity =>
            {
                entity.ToTable("merchOrderIndex");

                entity.HasIndex(e => e.OrderKey)
                    .HasName("IX_merchOrderIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.OrderKey).HasColumnName("orderKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.OrderKeyNavigation)
                    .WithOne(p => p.MerchOrderIndex)
                    .HasForeignKey<MerchOrderIndex>(d => d.OrderKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchOrderIndex_merchOrder");
            });

            modelBuilder.Entity<MerchOrderItem>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchOrderItem");

                entity.ToTable("merchOrderItem");

                entity.HasIndex(e => e.Sku)
                    .HasName("IX_merchOrderItemSku");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.BackOrder).HasColumnName("backOrder");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Exported).HasColumnName("exported");

                entity.Property(e => e.ExtendedData)
                    .HasColumnName("extendedData")
                    .HasColumnType("ntext");

                entity.Property(e => e.LineItemTfKey).HasColumnName("lineItemTfKey");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.OrderKey).HasColumnName("orderKey");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.Property(e => e.ShipmentKey).HasColumnName("shipmentKey");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.OrderKeyNavigation)
                    .WithMany(p => p.MerchOrderItem)
                    .HasForeignKey(d => d.OrderKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchOrderItem_merchOrder");

                entity.HasOne(d => d.ShipmentKeyNavigation)
                    .WithMany(p => p.MerchOrderItem)
                    .HasForeignKey(d => d.ShipmentKey)
                    .HasConstraintName("FK_merchOrderItem_merchShipment");
            });

            modelBuilder.Entity<MerchOrderStatus>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchOrderStatus");

                entity.ToTable("merchOrderStatus");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasColumnName("alias")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Reportable).HasColumnName("reportable");

                entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchPayment>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchPayment");

                entity.ToTable("merchPayment");

                entity.HasIndex(e => e.CustomerKey)
                    .HasName("IX_merchPaymentCustomer");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("numeric");

                entity.Property(e => e.Authorized)
                    .HasColumnName("authorized")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Collected)
                    .HasColumnName("collected")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                entity.Property(e => e.Exported)
                    .HasColumnName("exported")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ExtendedData)
                    .HasColumnName("extendedData")
                    .HasColumnType("ntext");

                entity.Property(e => e.PaymentMethodKey).HasColumnName("paymentMethodKey");

                entity.Property(e => e.PaymentMethodName)
                    .HasColumnName("paymentMethodName")
                    .HasMaxLength(255);

                entity.Property(e => e.PaymentTfKey).HasColumnName("paymentTfKey");

                entity.Property(e => e.ReferenceNumber)
                    .HasColumnName("referenceNumber")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Voided)
                    .HasColumnName("voided")
                    .HasDefaultValueSql("'0'");

                entity.HasOne(d => d.CustomerDtoKeyNavigation)
                    .WithMany(p => p.MerchPayment)
                    .HasForeignKey(d => d.CustomerKey)
                    .HasConstraintName("FK_merchPayment_merchCustomer");

                entity.HasOne(d => d.PaymentMethodKeyNavigation)
                    .WithMany(p => p.MerchPayment)
                    .HasForeignKey(d => d.PaymentMethodKey)
                    .HasConstraintName("FK_merchPayment_merchPaymentMethod");
            });

            modelBuilder.Entity<MerchPaymentMethod>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchPaymentMethod");

                entity.ToTable("merchPaymentMethod");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.PaymentCode)
                    .IsRequired()
                    .HasColumnName("paymentCode")
                    .HasMaxLength(255);

                entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.ProviderKeyNavigation)
                    .WithMany(p => p.MerchPaymentMethod)
                    .HasForeignKey(d => d.ProviderKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchPaymentMethod_merchGatewayProviderSettings");
            });

            modelBuilder.Entity<MerchProduct>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchProduct");

                entity.ToTable("merchProduct");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchProduct2EntityCollection>(entity =>
            {
                entity.HasKey(e => new { e.ProductKey, e.EntityCollectionKey })
                    .HasName("PK_merchProduct2EntityCollection");

                entity.ToTable("merchProduct2EntityCollection");

                entity.Property(e => e.ProductKey).HasColumnName("productKey");

                entity.Property(e => e.EntityCollectionKey).HasColumnName("entityCollectionKey");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.EntityCollectionDtoKeyNavigation)
                    .WithMany(p => p.MerchProduct2EntityCollection)
                    .HasForeignKey(d => d.EntityCollectionKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProduct2EntityCollection_merchEntityCollection");

                entity.HasOne(d => d.ProductKeyNavigation)
                    .WithMany(p => p.MerchProduct2EntityCollection)
                    .HasForeignKey(d => d.ProductKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProduct2EnityCollection_merchProduct");
            });

            modelBuilder.Entity<MerchProduct2ProductOption>(entity =>
            {
                entity.HasKey(e => new { e.ProductKey, e.OptionKey })
                    .HasName("PK_merchProduct2Option");

                entity.ToTable("merchProduct2ProductOption");

                entity.Property(e => e.ProductKey).HasColumnName("productKey");

                entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.UseName)
                    .HasColumnName("useName")
                    .HasMaxLength(255);

                entity.HasOne(d => d.OptionKeyNavigation)
                    .WithMany(p => p.MerchProduct2ProductOption)
                    .HasForeignKey(d => d.OptionKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProduct2Option_merchOption");

                entity.HasOne(d => d.ProductKeyNavigation)
                    .WithMany(p => p.MerchProduct2ProductOption)
                    .HasForeignKey(d => d.ProductKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProduct2Option_merchProduct");
            });

            modelBuilder.Entity<MerchProductAttribute>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchProductAttribute");

                entity.ToTable("merchProductAttribute");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.DetachedContentValues)
                    .HasColumnName("detachedContentValues")
                    .HasColumnType("ntext");

                entity.Property(e => e.IsDefaultChoice)
                    .HasColumnName("isDefaultChoice")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(255);

                entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.OptionKeyNavigation)
                    .WithMany(p => p.MerchProductAttribute)
                    .HasForeignKey(d => d.OptionKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductAttribute_merchOption");
            });

            modelBuilder.Entity<MerchProductOption>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchProductOption");

                entity.ToTable("merchProductOption");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.DetachedContentTypeKey).HasColumnName("detachedContentTypeKey");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Required)
                    .HasColumnName("required")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Shared)
                    .HasColumnName("shared")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UiOption)
                    .HasColumnName("uiOption")
                    .HasMaxLength(50);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.DetachedContentTypeDtoKeyNavigation)
                    .WithMany(p => p.MerchProductOption)
                    .HasForeignKey(d => d.DetachedContentTypeKey)
                    .HasConstraintName("FK_merchProductOptionDetachedContent_merchProductOption");
            });

            modelBuilder.Entity<MerchProductOptionAttributeShare>(entity =>
            {
                entity.HasKey(e => new { e.ProductKey, e.OptionKey, e.AttributeKey })
                    .HasName("PK_merchProductOptionAttributeShare");

                entity.ToTable("merchProductOptionAttributeShare");

                entity.Property(e => e.ProductKey).HasColumnName("productKey");

                entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                entity.Property(e => e.AttributeKey).HasColumnName("attributeKey");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.IsDefaultChoice).HasColumnName("isDefaultChoice");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.AttributeKeyNavigation)
                    .WithMany(p => p.MerchProductOptionAttributeShare)
                    .HasForeignKey(d => d.AttributeKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductOptionAttributeShare_merchProductAttribute");

                entity.HasOne(d => d.OptionKeyNavigation)
                    .WithMany(p => p.MerchProductOptionAttributeShare)
                    .HasForeignKey(d => d.OptionKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductOptionAttributeShare_merchProductOption");

                entity.HasOne(d => d.ProductKeyNavigation)
                    .WithMany(p => p.MerchProductOptionAttributeShare)
                    .HasForeignKey(d => d.ProductKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductOptionAttributeShare_merchProduct");
            });

            modelBuilder.Entity<MerchProductVariant>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchProductVariant");

                entity.ToTable("merchProductVariant");

                entity.HasIndex(e => e.Barcode)
                    .HasName("IX_merchProductVariantBarcode");

                entity.HasIndex(e => e.Manufacturer)
                    .HasName("IX_merchProductVariantManufacturer");

                entity.HasIndex(e => e.Name)
                    .HasName("IX_merchProductVariantName");

                entity.HasIndex(e => e.Price)
                    .HasName("IX_merchProductVariantPrice");

                entity.HasIndex(e => e.SalePrice)
                    .HasName("IX_merchProductVariantSalePrice");

                entity.HasIndex(e => e.Sku)
                    .HasName("IX_merchProductVariantSku")
                    .IsUnique();

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Available).HasColumnName("available");

                entity.Property(e => e.Barcode)
                    .HasColumnName("barcode")
                    .HasMaxLength(255);

                entity.Property(e => e.CostOfGoods)
                    .HasColumnName("costOfGoods")
                    .HasColumnType("numeric");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Download)
                    .HasColumnName("download")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DownloadMediaId).HasColumnName("downloadMediaId");

                entity.Property(e => e.Height)
                    .HasColumnName("height")
                    .HasColumnType("numeric");

                entity.Property(e => e.Length)
                    .HasColumnName("length")
                    .HasColumnType("numeric");

                entity.Property(e => e.Manufacturer)
                    .HasColumnName("manufacturer")
                    .HasMaxLength(255);

                entity.Property(e => e.Master)
                    .HasColumnName("master")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ModelNumber)
                    .HasColumnName("modelNumber")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.OnSale).HasColumnName("onSale");

                entity.Property(e => e.OutOfStockPurchase).HasColumnName("outOfStockPurchase");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("numeric");

                entity.Property(e => e.ProductKey).HasColumnName("productKey");

                entity.Property(e => e.SalePrice)
                    .HasColumnName("salePrice")
                    .HasColumnType("numeric");

                entity.Property(e => e.Shippable)
                    .HasColumnName("shippable")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.Sku)
                    .IsRequired()
                    .HasColumnName("sku")
                    .HasMaxLength(255);

                entity.Property(e => e.Taxable)
                    .HasColumnName("taxable")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.TrackInventory)
                    .HasColumnName("trackInventory")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.VersionKey)
                    .HasColumnName("versionKey")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Weight)
                    .HasColumnName("weight")
                    .HasColumnType("numeric");

                entity.Property(e => e.Width)
                    .HasColumnName("width")
                    .HasColumnType("numeric");

                entity.HasOne(d => d.ProductKeyNavigation)
                    .WithMany(p => p.MerchProductVariant)
                    .HasForeignKey(d => d.ProductKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductVariant_merchProduct");
            });

            modelBuilder.Entity<MerchProductVariant2ProductAttribute>(entity =>
            {
                entity.HasKey(e => new { e.ProductVariantKey, e.OptionKey })
                    .HasName("PK_merchProductVariant2ProductAttribute");

                entity.ToTable("merchProductVariant2ProductAttribute");

                entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ProductAttributeKey).HasColumnName("productAttributeKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.OptionKeyNavigation)
                    .WithMany(p => p.MerchProductVariant2ProductAttribute)
                    .HasForeignKey(d => d.OptionKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductVariant2ProductAttribute_merchProductOption");

                entity.HasOne(d => d.ProductAttributeKeyNavigation)
                    .WithMany(p => p.MerchProductVariant2ProductAttribute)
                    .HasForeignKey(d => d.ProductAttributeKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductVariant2ProductAttribute_merchProductAttribute");

                entity.HasOne(d => d.ProductVariantKeyNavigation)
                    .WithMany(p => p.MerchProductVariant2ProductAttribute)
                    .HasForeignKey(d => d.ProductVariantKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductVariant2ProductAttribute_merchProductVariant");
            });

            modelBuilder.Entity<MerchProductVariantDetachedContent>(entity =>
            {
                entity.HasKey(e => new { e.ProductVariantKey, e.CultureName })
                    .HasName("PK_merchProductVariantDetachedContent");

                entity.ToTable("merchProductVariantDetachedContent");

                entity.HasIndex(e => e.CultureName)
                    .HasName("IX_merchProductVariantDetachedContentCultureName");

                entity.HasIndex(e => e.Pk)
                    .HasName("IX_merchProductVariantDetachedContentKey")
                    .IsUnique();

                entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                entity.Property(e => e.CultureName)
                    .HasColumnName("cultureName")
                    .HasMaxLength(255);

                entity.Property(e => e.CanBeRendered)
                    .HasColumnName("canBeRendered")
                    .HasDefaultValueSql("'1'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.DetachedContentTypeKey).HasColumnName("detachedContentTypeKey");

                entity.Property(e => e.Pk).HasColumnName("pk");

                entity.Property(e => e.Slug)
                    .HasColumnName("slug")
                    .HasMaxLength(255);

                entity.Property(e => e.TemplateId).HasColumnName("templateId");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Values)
                    .HasColumnName("values")
                    .HasColumnType("ntext");

                entity.HasOne(d => d.DetachedContentTypeDtoKeyNavigation)
                    .WithMany(p => p.MerchProductVariantDetachedContent)
                    .HasForeignKey(d => d.DetachedContentTypeKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductVariantDetachedContent_merchDetachedContentTypeKey");

                entity.HasOne(d => d.ProductVariantKeyNavigation)
                    .WithMany(p => p.MerchProductVariantDetachedContent)
                    .HasForeignKey(d => d.ProductVariantKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductVariantDetachedContent_merchProductVariant");
            });

            modelBuilder.Entity<MerchProductVariantIndex>(entity =>
            {
                entity.ToTable("merchProductVariantIndex");

                entity.HasIndex(e => e.ProductVariantKey)
                    .HasName("IX_merchProductVariantIndex")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.ProductVariantKeyNavigation)
                    .WithOne(p => p.MerchProductVariantIndex)
                    .HasForeignKey<MerchProductVariantIndex>(d => d.ProductVariantKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchProductVariantIndex_merchProductVariant");
            });

            modelBuilder.Entity<MerchShipCountry>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchShipCountry");

                entity.ToTable("merchShipCountry");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CatalogKey).HasColumnName("catalogKey");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnName("countryCode")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.CatalogKeyNavigation)
                    .WithMany(p => p.MerchShipCountry)
                    .HasForeignKey(d => d.CatalogKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchCatalogCountry_merchWarehouseCatalog");
            });

            modelBuilder.Entity<MerchShipMethod>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchShipMethod");

                entity.ToTable("merchShipMethod");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                entity.Property(e => e.ProvinceData)
                    .HasColumnName("provinceData")
                    .HasColumnType("ntext");

                entity.Property(e => e.ServiceCode)
                    .HasColumnName("serviceCode")
                    .HasMaxLength(255);

                entity.Property(e => e.ShipCountryKey).HasColumnName("shipCountryKey");

                entity.Property(e => e.Surcharge)
                    .HasColumnName("surcharge")
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.Taxable).HasColumnName("taxable");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.ProviderKeyNavigation)
                    .WithMany(p => p.MerchShipMethod)
                    .HasForeignKey(d => d.ProviderKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchShipMethod_merchGatewayProviderSettings");

                entity.HasOne(d => d.ShipCountryKeyNavigation)
                    .WithMany(p => p.MerchShipMethod)
                    .HasForeignKey(d => d.ShipCountryKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchShipMethod_merchShipCountry");
            });

            modelBuilder.Entity<MerchShipRateTier>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchShipRateTier");

                entity.ToTable("merchShipRateTier");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.RangeHigh)
                    .HasColumnName("rangeHigh")
                    .HasColumnType("numeric");

                entity.Property(e => e.RangeLow)
                    .HasColumnName("rangeLow")
                    .HasColumnType("numeric");

                entity.Property(e => e.Rate)
                    .HasColumnName("rate")
                    .HasColumnType("numeric");

                entity.Property(e => e.ShipMethodKey).HasColumnName("shipMethodKey");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.ShipMethodKeyNavigation)
                    .WithMany(p => p.MerchShipRateTier)
                    .HasForeignKey(d => d.ShipMethodKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchShipmentRateTier_merchShipMethod");
            });

            modelBuilder.Entity<MerchShipment>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchShipment");

                entity.ToTable("merchShipment");

                entity.HasIndex(e => e.ShipmentNumber)
                    .HasName("IX_merchShipmentNumber")
                    .IsUnique();

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Carrier)
                    .HasColumnName("carrier")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.FromAddress1)
                    .HasColumnName("fromAddress1")
                    .HasMaxLength(255);

                entity.Property(e => e.FromAddress2)
                    .HasColumnName("fromAddress2")
                    .HasMaxLength(255);

                entity.Property(e => e.FromCountryCode)
                    .HasColumnName("fromCountryCode")
                    .HasMaxLength(255);

                entity.Property(e => e.FromIsCommercial).HasColumnName("fromIsCommercial");

                entity.Property(e => e.FromLocality)
                    .HasColumnName("fromLocality")
                    .HasMaxLength(255);

                entity.Property(e => e.FromName)
                    .HasColumnName("fromName")
                    .HasMaxLength(255);

                entity.Property(e => e.FromOrganization)
                    .HasColumnName("fromOrganization")
                    .HasMaxLength(255);

                entity.Property(e => e.FromPostalCode)
                    .HasColumnName("fromPostalCode")
                    .HasMaxLength(255);

                entity.Property(e => e.FromRegion)
                    .HasColumnName("fromRegion")
                    .HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(255);

                entity.Property(e => e.ShipMethodKey).HasColumnName("shipMethodKey");

                entity.Property(e => e.ShipmentNumber).HasColumnName("shipmentNumber");

                entity.Property(e => e.ShipmentNumberPrefix)
                    .HasColumnName("shipmentNumberPrefix")
                    .HasMaxLength(255);

                entity.Property(e => e.ShipmentStatusKey).HasColumnName("shipmentStatusKey");

                entity.Property(e => e.ShippedDate)
                    .HasColumnName("shippedDate")
                    .HasColumnType("datetime");

                entity.Property(e => e.ToAddress1)
                    .HasColumnName("toAddress1")
                    .HasMaxLength(255);

                entity.Property(e => e.ToAddress2)
                    .HasColumnName("toAddress2")
                    .HasMaxLength(255);

                entity.Property(e => e.ToCountryCode)
                    .HasColumnName("toCountryCode")
                    .HasMaxLength(255);

                entity.Property(e => e.ToIsCommercial).HasColumnName("toIsCommercial");

                entity.Property(e => e.ToLocality)
                    .HasColumnName("toLocality")
                    .HasMaxLength(255);

                entity.Property(e => e.ToName)
                    .HasColumnName("toName")
                    .HasMaxLength(255);

                entity.Property(e => e.ToOrganization)
                    .HasColumnName("toOrganization")
                    .HasMaxLength(255);

                entity.Property(e => e.ToPostalCode)
                    .HasColumnName("toPostalCode")
                    .HasMaxLength(255);

                entity.Property(e => e.ToRegion)
                    .HasColumnName("toRegion")
                    .HasMaxLength(255);

                entity.Property(e => e.TrackingCode)
                    .HasColumnName("trackingCode")
                    .HasMaxLength(255);

                entity.Property(e => e.TrackingUrl)
                    .HasColumnName("trackingUrl")
                    .HasMaxLength(1000);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.VersionKey)
                    .HasColumnName("versionKey")
                    .HasDefaultValueSql("'newid()'");

                entity.HasOne(d => d.ShipMethodKeyNavigation)
                    .WithMany(p => p.MerchShipment)
                    .HasForeignKey(d => d.ShipMethodKey)
                    .HasConstraintName("FK_merchShipment_merchShipMethod");

                entity.HasOne(d => d.ShipmentStatusKeyNavigation)
                    .WithMany(p => p.MerchShipment)
                    .HasForeignKey(d => d.ShipmentStatusKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchShipment_merchShipmentStatus");
            });

            modelBuilder.Entity<MerchShipmentStatus>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchShipmentStatus");

                entity.ToTable("merchShipmentStatus");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Active).HasColumnName("active");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasColumnName("alias")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Reportable).HasColumnName("reportable");

                entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchStoreSetting>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchStoreSetting");

                entity.ToTable("merchStoreSetting");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.TypeName)
                    .IsRequired()
                    .HasColumnName("typeName")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value")
                    .HasMaxLength(255);
            });

            modelBuilder.Entity<MerchTaxMethod>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchTaxMethod");

                entity.ToTable("merchTaxMethod");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CountryCode)
                    .IsRequired()
                    .HasColumnName("countryCode")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.PercentageTaxRate)
                    .HasColumnName("percentageTaxRate")
                    .HasColumnType("numeric")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.ProductTaxMethod).HasColumnName("productTaxMethod");

                entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                entity.Property(e => e.ProvinceData)
                    .HasColumnName("provinceData")
                    .HasColumnType("ntext");

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.HasOne(d => d.ProviderKeyNavigation)
                    .WithMany(p => p.MerchTaxMethod)
                    .HasForeignKey(d => d.ProviderKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchTaxMethod_merchGatewayProviderSettings");
            });

            modelBuilder.Entity<MerchTypeField>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchTypeField");

                entity.ToTable("merchTypeField");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Alias)
                    .IsRequired()
                    .HasColumnName("alias")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchWarehouse>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchWarehouse");

                entity.ToTable("merchWarehouse");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.Address1)
                    .HasColumnName("address1")
                    .HasMaxLength(255);

                entity.Property(e => e.Address2)
                    .HasColumnName("address2")
                    .HasMaxLength(255);

                entity.Property(e => e.CountryCode)
                    .HasColumnName("countryCode")
                    .HasMaxLength(255);

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasMaxLength(255);

                entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                entity.Property(e => e.Locality)
                    .HasColumnName("locality")
                    .HasMaxLength(255);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(255);

                entity.Property(e => e.PostalCode)
                    .HasColumnName("postalCode")
                    .HasMaxLength(255);

                entity.Property(e => e.Region)
                    .HasColumnName("region")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");
            });

            modelBuilder.Entity<MerchWarehouseCatalog>(entity =>
            {
                entity.HasKey(e => e.Pk)
                    .HasName("PK_merchWarehouseCatalog");

                entity.ToTable("merchWarehouseCatalog");

                entity.Property(e => e.Pk)
                    .HasColumnName("pk")
                    .HasDefaultValueSql("'newid()'");

                entity.Property(e => e.CreateDate)
                    .HasColumnName("createDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(500);

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasMaxLength(255);

                entity.Property(e => e.UpdateDate)
                    .HasColumnName("updateDate")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("getdate()");

                entity.Property(e => e.WarehouseKey).HasColumnName("warehouseKey");

                entity.HasOne(d => d.WarehouseKeyNavigation)
                    .WithMany(p => p.MerchWarehouseCatalog)
                    .HasForeignKey(d => d.WarehouseKey)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_merchWarehouseCatalog_merchWarehouse");
            });
        }
    }
}