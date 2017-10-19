namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchPaymentMethodDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchPaymentMethod>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchPaymentMethod");

                    entity.ToTable("merchPaymentMethod");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Description)
                        .HasColumnName("description")
                        .HasMaxLength(255);

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.PaymentCode)
                        .IsRequired()
                        .HasColumnName("paymentCode")
                        .HasMaxLength(255);

                    entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.ProviderKeyNavigation)
                        .WithMany(p => p.MerchPaymentMethod)
                        .HasForeignKey(d => d.ProviderKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchPaymentMethod_merchGatewayProviderSettings");
                });
        }
    }
}
