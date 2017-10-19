namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchShipRateTierDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
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
                }); ;
        }
    }
}