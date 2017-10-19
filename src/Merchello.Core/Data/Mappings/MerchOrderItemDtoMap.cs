namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchOrderItemDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
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
        }
    }
}