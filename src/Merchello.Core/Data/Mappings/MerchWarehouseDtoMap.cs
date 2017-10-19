namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchWarehouseDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchWarehouse>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchWarehouse");

                        entity.ToTable("merchWarehouse");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.Address1).HasColumnName("address1").HasMaxLength(255);

                        entity.Property(e => e.Address2).HasColumnName("address2").HasMaxLength(255);

                        entity.Property(e => e.CountryCode).HasColumnName("countryCode").HasMaxLength(255);

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);

                        entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                        entity.Property(e => e.Locality).HasColumnName("locality").HasMaxLength(255);

                        entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasMaxLength(255);

                        entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(255);

                        entity.Property(e => e.PostalCode).HasColumnName("postalCode").HasMaxLength(255);

                        entity.Property(e => e.Region).HasColumnName("region").HasMaxLength(255);

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");
                    });
        }
    }
}