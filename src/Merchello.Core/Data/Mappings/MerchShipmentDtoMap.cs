namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchShipmentDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchShipment>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchShipment");

                        entity.ToTable("merchShipment");

                        entity.HasIndex(e => e.ShipmentNumber).HasName("IX_merchShipmentNumber").IsUnique();

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.Carrier).HasColumnName("carrier").HasMaxLength(255);

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(255);

                        entity.Property(e => e.FromAddress1).HasColumnName("fromAddress1").HasMaxLength(255);

                        entity.Property(e => e.FromAddress2).HasColumnName("fromAddress2").HasMaxLength(255);

                        entity.Property(e => e.FromCountryCode).HasColumnName("fromCountryCode").HasMaxLength(255);

                        entity.Property(e => e.FromIsCommercial).HasColumnName("fromIsCommercial");

                        entity.Property(e => e.FromLocality).HasColumnName("fromLocality").HasMaxLength(255);

                        entity.Property(e => e.FromName).HasColumnName("fromName").HasMaxLength(255);

                        entity.Property(e => e.FromOrganization).HasColumnName("fromOrganization").HasMaxLength(255);

                        entity.Property(e => e.FromPostalCode).HasColumnName("fromPostalCode").HasMaxLength(255);

                        entity.Property(e => e.FromRegion).HasColumnName("fromRegion").HasMaxLength(255);

                        entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(255);

                        entity.Property(e => e.ShipMethodKey).HasColumnName("shipMethodKey");

                        entity.Property(e => e.ShipmentNumber).HasColumnName("shipmentNumber");

                        entity.Property(e => e.ShipmentNumberPrefix)
                            .HasColumnName("shipmentNumberPrefix")
                            .HasMaxLength(255);

                        entity.Property(e => e.ShipmentStatusKey).HasColumnName("shipmentStatusKey");

                        entity.Property(e => e.ShippedDate).HasColumnName("shippedDate").HasColumnType("datetime");

                        entity.Property(e => e.ToAddress1).HasColumnName("toAddress1").HasMaxLength(255);

                        entity.Property(e => e.ToAddress2).HasColumnName("toAddress2").HasMaxLength(255);

                        entity.Property(e => e.ToCountryCode).HasColumnName("toCountryCode").HasMaxLength(255);

                        entity.Property(e => e.ToIsCommercial).HasColumnName("toIsCommercial");

                        entity.Property(e => e.ToLocality).HasColumnName("toLocality").HasMaxLength(255);

                        entity.Property(e => e.ToName).HasColumnName("toName").HasMaxLength(255);

                        entity.Property(e => e.ToOrganization).HasColumnName("toOrganization").HasMaxLength(255);

                        entity.Property(e => e.ToPostalCode).HasColumnName("toPostalCode").HasMaxLength(255);

                        entity.Property(e => e.ToRegion).HasColumnName("toRegion").HasMaxLength(255);

                        entity.Property(e => e.TrackingCode).HasColumnName("trackingCode").HasMaxLength(255);

                        entity.Property(e => e.TrackingUrl).HasColumnName("trackingUrl").HasMaxLength(1000);

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.VersionKey).HasColumnName("versionKey").HasDefaultValueSql("'newid()'");

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
        }
    }
}