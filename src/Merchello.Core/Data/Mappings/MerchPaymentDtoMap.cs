namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchPaymentDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchPayment>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchPayment");

                        entity.ToTable("merchPayment");

                        entity.HasIndex(e => e.CustomerKey).HasName("IX_merchPaymentCustomer");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.Amount).HasColumnName("amount").HasColumnType("numeric");

                        entity.Property(e => e.Authorized).HasColumnName("authorized").HasDefaultValueSql("'1'");

                        entity.Property(e => e.Collected).HasColumnName("collected").HasDefaultValueSql("'1'");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                        entity.Property(e => e.Exported).HasColumnName("exported").HasDefaultValueSql("'0'");

                        entity.Property(e => e.ExtendedData).HasColumnName("extendedData").HasColumnType("ntext");

                        entity.Property(e => e.PaymentMethodKey).HasColumnName("paymentMethodKey");

                        entity.Property(e => e.PaymentMethodName).HasColumnName("paymentMethodName").HasMaxLength(255);

                        entity.Property(e => e.PaymentTfKey).HasColumnName("paymentTfKey");

                        entity.Property(e => e.ReferenceNumber).HasColumnName("referenceNumber").HasMaxLength(255);

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Voided).HasColumnName("voided").HasDefaultValueSql("'0'");

                        entity.HasOne(d => d.CustomerDtoKeyNavigation)
                            .WithMany(p => p.MerchPayment)
                            .HasForeignKey(d => d.CustomerKey)
                            .HasConstraintName("FK_merchPayment_merchCustomer");

                        entity.HasOne(d => d.PaymentMethodKeyNavigation)
                            .WithMany(p => p.MerchPayment)
                            .HasForeignKey(d => d.PaymentMethodKey)
                            .HasConstraintName("FK_merchPayment_merchPaymentMethod");
                    });
        }
    }
}