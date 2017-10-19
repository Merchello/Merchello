namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchOfferRedeemedDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OfferRedeemedDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchOfferRedeemed");

                    entity.ToTable("merchOfferRedeemed");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.InvoiceKey).HasColumnName("invoiceKey");

                    entity.Property(e => e.OfferCode)
                        .IsRequired()
                        .HasColumnName("offerCode")
                        .HasMaxLength(255);

                    entity.Property(e => e.OfferProviderKey).HasColumnName("offerProviderKey");

                    entity.Property(e => e.OfferSettingsKey).HasColumnName("offerSettingsKey");

                    entity.Property(e => e.RedeemedDate)
                        .HasColumnName("redeemedDate")
                        .HasColumnType("datetime");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.InvoiceDtoKeyNavigation)
                        .WithMany(p => p.MerchOfferRedeemed)
                        .HasForeignKey(d => d.InvoiceKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchOfferRedeemed_merchInvoice");

                    entity.HasOne(d => d.OfferSettingsDtoKeyNavigation)
                        .WithMany(p => p.MerchOfferRedeemed)
                        .HasForeignKey(d => d.OfferSettingsKey)
                        .HasConstraintName("FK_merchOfferRedeemed_merchOfferSettings");
                });
        }
    }
}
