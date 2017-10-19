namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProductVariant2ProductAttributeDtoMap : IEntityMap

    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariant2ProductAttributeDto>(entity =>
                {
                    entity.HasKey(e => new { e.ProductVariantKey, e.OptionKey })
                        .HasName("PK_merchProductVariant2ProductAttribute");

                    entity.ToTable("merchProductVariant2ProductAttribute");

                    entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                    entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.ProductAttributeKey).HasColumnName("productAttributeKey");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.OptionDtoKeyNavigation)
                        .WithMany(p => p.MerchProductVariant2ProductAttribute)
                        .HasForeignKey(d => d.OptionKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProductVariant2ProductAttribute_merchProductOption");

                    entity.HasOne(d => d.ProductAttributeDtoKeyNavigation)
                        .WithMany(p => p.MerchProductVariant2ProductAttribute)
                        .HasForeignKey(d => d.ProductAttributeKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProductVariant2ProductAttribute_merchProductAttribute");

                    entity.HasOne(d => d.ProductVariantDtoKeyNavigation)
                        .WithMany(p => p.MerchProductVariant2ProductAttribute)
                        .HasForeignKey(d => d.ProductVariantKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProductVariant2ProductAttribute_merchProductVariant");
                });
        }
    }
}
