namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProductVariantDetachedContentDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductVariantDetachedContentDto>(entity =>
                {
                    entity.HasKey(e => new { e.ProductVariantKey, e.CultureName })
                        .HasName("PK_merchProductVariantDetachedContent");

                    entity.ToTable("merchProductVariantDetachedContent");

                    entity.HasIndex(e => e.CultureName)
                        .HasName("IX_merchProductVariantDetachedContentCultureName");

                    entity.HasIndex(e => e.Pk)
                        .HasName("IX_merchProductVariantDetachedContentKey")
                        .IsUnique();

                    entity.Property(e => e.ProductVariantKey).HasColumnName("productVariantKey");

                    entity.Property(e => e.CultureName)
                        .HasColumnName("cultureName")
                        .HasMaxLength(255);

                    entity.Property(e => e.CanBeRendered)
                        .HasColumnName("canBeRendered")
                        .HasDefaultValueSql("'1'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.DetachedContentTypeKey).HasColumnName("detachedContentTypeKey");

                    entity.Property(e => e.Pk).HasColumnName("pk");

                    entity.Property(e => e.Slug)
                        .HasColumnName("slug")
                        .HasMaxLength(255);

                    entity.Property(e => e.TemplateId).HasColumnName("templateId");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Values)
                        .HasColumnName("values")
                        .HasColumnType("ntext");

                    entity.HasOne(d => d.DetachedContentTypeDtoKeyNavigation)
                        .WithMany(p => p.MerchProductVariantDetachedContent)
                        .HasForeignKey(d => d.DetachedContentTypeKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProductVariantDetachedContent_merchDetachedContentTypeKey");

                    entity.HasOne(d => d.ProductVariantDtoKeyNavigation)
                        .WithMany(p => p.MerchProductVariantDetachedContent)
                        .HasForeignKey(d => d.ProductVariantKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProductVariantDetachedContent_merchProductVariant");
                });
        }
    }
}