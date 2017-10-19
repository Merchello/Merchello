namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchShipMethodDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchShipMethod>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchShipMethod");

                        entity.ToTable("merchShipMethod");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasMaxLength(255);

                        entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                        entity.Property(e => e.ProvinceData).HasColumnName("provinceData").HasColumnType("ntext");

                        entity.Property(e => e.ServiceCode).HasColumnName("serviceCode").HasMaxLength(255);

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
        }
    }
}