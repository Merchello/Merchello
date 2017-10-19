namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchShipCountryDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShipCountryDto>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchShipCountry");

                        entity.ToTable("merchShipCountry");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.CatalogKey).HasColumnName("catalogKey");

                        entity.Property(e => e.CountryCode).IsRequired().HasColumnName("countryCode").HasMaxLength(255);

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasMaxLength(255);

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.HasOne(d => d.CatalogDtoKeyNavigation)
                            .WithMany(p => p.MerchShipCountry)
                            .HasForeignKey(d => d.CatalogKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchCatalogCountry_merchWarehouseCatalog");
                    });
        }
    }
}