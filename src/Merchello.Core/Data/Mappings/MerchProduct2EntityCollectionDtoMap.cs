namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProduct2EntityCollectionDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product2EntityCollectionDto>(entity =>
                {
                    entity.HasKey(e => new { e.ProductKey, e.EntityCollectionKey })
                        .HasName("PK_merchProduct2EntityCollection");

                    entity.ToTable("merchProduct2EntityCollection");

                    entity.Property(e => e.ProductKey).HasColumnName("productKey");

                    entity.Property(e => e.EntityCollectionKey).HasColumnName("entityCollectionKey");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.EntityCollectionDtoKeyNavigation)
                        .WithMany(p => p.MerchProduct2EntityCollection)
                        .HasForeignKey(d => d.EntityCollectionKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProduct2EntityCollection_merchEntityCollection");

                    entity.HasOne(d => d.ProductDtoKeyNavigation)
                        .WithMany(p => p.MerchProduct2EntityCollection)
                        .HasForeignKey(d => d.ProductKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProduct2EnityCollection_merchProduct");
                });
        }
    }
}
