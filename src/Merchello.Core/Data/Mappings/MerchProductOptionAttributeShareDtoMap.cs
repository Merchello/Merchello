namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProductOptionAttributeShareDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductOptionAttributeShareDto>(
                entity =>
                    {
                        entity.HasKey(e => new { e.ProductKey, e.OptionKey, e.AttributeKey })
                            .HasName("PK_merchProductOptionAttributeShare");

                        entity.ToTable("merchProductOptionAttributeShare");

                        entity.Property(e => e.ProductKey).HasColumnName("productKey");

                        entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                        entity.Property(e => e.AttributeKey).HasColumnName("attributeKey");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.IsDefaultChoice).HasColumnName("isDefaultChoice");

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.HasOne(d => d.AttributeDtoKeyNavigation)
                            .WithMany(p => p.MerchProductOptionAttributeShare)
                            .HasForeignKey(d => d.AttributeKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchProductOptionAttributeShare_merchProductAttribute");

                        entity.HasOne(d => d.OptionDtoKeyNavigation)
                            .WithMany(p => p.MerchProductOptionAttributeShare)
                            .HasForeignKey(d => d.OptionKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchProductOptionAttributeShare_merchProductOption");

                        entity.HasOne(d => d.ProductDtoKeyNavigation)
                            .WithMany(p => p.MerchProductOptionAttributeShare)
                            .HasForeignKey(d => d.ProductKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchProductOptionAttributeShare_merchProduct");
                    });
        }
    }
}