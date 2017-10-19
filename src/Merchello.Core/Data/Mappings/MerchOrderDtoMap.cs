namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchOrderDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
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
        }
    }
}