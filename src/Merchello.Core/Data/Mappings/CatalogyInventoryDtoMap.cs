namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class CatalogyInventoryDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CatalogInventoryDto>(entity =>
                {
                    entity.ToTable("merchCatalogInventory");

                    entity.HasKey(e => new { e.CatalogKey, e.ProductVariantKey })
                        .HasName("PK_merchCatalogInventory");

                    entity.HasIndex(e => e.Location)
                        .HasName("IX_merchCatalogInventoryLocation");

                    entity.Property(e => e.CatalogKey).HasColumnName("catalogKey");

                    entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                    entity.Property(e => e.Count).HasColumnName("count");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Location)
                        .HasColumnName("location")
                        .HasMaxLength(255);

                    entity.Property(e => e.LowCount).HasColumnName("lowCount");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.CatalogDtoKeyNavigation)
                        .WithMany(p => p.MerchCatalogInventory)
                        .HasForeignKey(d => d.CatalogKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchCatalogInventory_merchWarehouseCatalog");

                    entity.HasOne(d => d.ProductVariantDtoKeyNavigation)
                        .WithMany(p => p.MerchCatalogInventory)
                        .HasForeignKey(d => d.ProductVariantKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchWarehouseInventory_merchProductVariant");
                });
        }
    }
}