namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProductVariantDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariantDto>(entity =>
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

                    entity.HasOne(d => d.ProductDtoKeyNavigation)
                        .WithMany(p => p.MerchProductVariant)
                        .HasForeignKey(d => d.ProductKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProductVariant_merchProduct");
                });
        }
    }
}