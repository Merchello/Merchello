namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class AppliedPaymentDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppliedPaymentDto>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchAppliedPayment");

                        entity.ToTable("merchAppliedPayment");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("numeric");

                        entity.Property(e => e.AppliedPaymentTfKey).HasColumnName("appliedPaymentTfKey");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Description).IsRequired().HasColumnName("description").HasMaxLength(500);

                        entity.Property(e => e.Exported).HasColumnName("exported");

                        entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                        entity.Property(e => e.PaymentKey).HasColumnName("paymentKey");

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                            .WithMany(p => p.MerchAppliedPayment)
                            .HasForeignKey(d => d.InvoiceKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchAppliedPayment_merchInvoice");

                        entity.HasOne(d => d.PaymentDtoKeyNavigation)
                            .WithMany(p => p.MerchAppliedPayment)
                            .HasForeignKey(d => d.PaymentKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchAppliedPayment_merchPayment");
                    });
        }
    }
}