namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class DigitalMediaDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DigitalMediaDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchDigitalMedia");

                    entity.ToTable("merchDigitalMedia");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.FirstAccessed)
                        .HasColumnName("firstAccessed")
                        .HasColumnType("datetime");

                    entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");
                });
        }
    }
}