namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class Invoice2EntityCollectionDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice2EntityCollectionDto>(entity =>
                {
                    entity.HasKey(e => new { e.InvoiceKey, e.EntityCollectionKey })
                        .HasName("PK_merchInvoice2EntityCollection");

                    entity.ToTable("merchInvoice2EntityCollection");

                    entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                    entity.Property(e => e.EntityCollectionKey).HasColumnName("entityCollectionKey");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.EntityCollectionDtoKeyNavigation)
                        .WithMany(p => p.MerchInvoice2EntityCollection)
                        .HasForeignKey(d => d.EntityCollectionKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchInvoice2EntityCollection_merchEntityCollection");

                    entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                        .WithMany(p => p.MerchInvoice2EntityCollection)
                        .HasForeignKey(d => d.InvoiceKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchInvoice2EntityCollection_merchInvoice");
                });
        }
    }
}