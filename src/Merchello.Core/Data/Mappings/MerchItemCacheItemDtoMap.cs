namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchItemCacheItemDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchItemCacheItem>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchItemCacheItem");

                    entity.ToTable("merchItemCacheItem");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Exported).HasColumnName("exported");

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.ItemCacheKey).HasColumnName("itemCacheKey");

                    entity.Property(e => e.LineItemTfKey).HasColumnName("lineItemTfKey");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.Price)
                        .HasColumnName("price")
                        .HasColumnType("numeric");

                    entity.Property(e => e.Quantity).HasColumnName("quantity");

                    entity.Property(e => e.Sku)
                        .IsRequired()
                        .HasColumnName("sku")
                        .HasMaxLength(255);

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.ItemCacheKeyNavigation)
                        .WithMany(p => p.MerchItemCacheItem)
                        .HasForeignKey(d => d.ItemCacheKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchItemCacheItem_merchItemCache");
                });
        }
    }
}