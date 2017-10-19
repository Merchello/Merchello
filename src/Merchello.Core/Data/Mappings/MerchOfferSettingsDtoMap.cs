namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchOfferSettingsDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchOfferSettings>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchOfferSettings");

                    entity.ToTable("merchOfferSettings");

                    entity.HasIndex(e => e.OfferCode)
                        .HasName("IX_merchOfferSettingsOfferCode")
                        .IsUnique();

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.Active).HasColumnName("active");

                    entity.Property(e => e.ConfigurationData)
                        .HasColumnName("configurationData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.OfferCode)
                        .IsRequired()
                        .HasColumnName("offerCode")
                        .HasMaxLength(255);

                    entity.Property(e => e.OfferEndsDate)
                        .HasColumnName("offerEndsDate")
                        .HasColumnType("datetime");

                    entity.Property(e => e.OfferProviderKey).HasColumnName("offerProviderKey");

                    entity.Property(e => e.OfferStartsDate)
                        .HasColumnName("offerStartsDate")
                        .HasColumnType("datetime");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");
                });
        }
    }
}
