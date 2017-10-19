namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProductVariantIndexDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariantIndexDto>(
                entity =>
                    {
                        entity.ToTable("merchProductVariantIndex");

                        entity.HasIndex(e => e.ProductVariantKey).HasName("IX_merchProductVariantIndex").IsUnique();

                        entity.Property(e => e.Id).HasColumnName("id");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        //entity.HasOne(d => d.ProductVariantKeyNavigation)
                        //    .WithOne(p => p.MerchProductVariantIndex)
                        //    .HasForeignKey<MerchProductVariantIndex>(d => d.ProductVariantKey)
                        //    .OnDelete(DeleteBehavior.Restrict)
                        //    .HasConstraintName("FK_merchProductVariantIndex_merchProductVariant");
                    });
        }
    }
}