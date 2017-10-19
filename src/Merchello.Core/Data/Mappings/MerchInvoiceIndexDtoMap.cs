namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchInvoiceIndexDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchInvoiceIndex>(
                entity =>
                    {
                        entity.ToTable("merchInvoiceIndex");

                        entity.HasIndex(e => e.InvoiceKey).HasName("IX_merchInvoiceIndex").IsUnique();

                        entity.Property(e => e.Id).HasColumnName("id");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        //entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                        //    .WithOne(p => p.MerchInvoiceIndex)
                        //    .HasForeignKey<MerchInvoiceIndex>(d => d.InvoiceKey)
                        //    .OnDelete(DeleteBehavior.Restrict)
                        //    .HasConstraintName("FK_merchInvoiceIndex_merchInvoice");
                    });
        }
    }
}