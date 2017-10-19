namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchWarehouseCatalogDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<WarehouseCatalogDto>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchWarehouseCatalog");

                        entity.ToTable("merchWarehouseCatalog");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Description).HasColumnName("description").HasMaxLength(500);

                        entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.WarehouseKey).HasColumnName("warehouseKey");

                        entity.HasOne(d => d.WarehouseDtoKeyNavigation)
                            .WithMany(p => p.MerchWarehouseCatalog)
                            .HasForeignKey(d => d.WarehouseKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchWarehouseCatalog_merchWarehouse");
                    });
        }
    }
}