namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProduct2ProductOptionDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchProduct2ProductOption>(entity =>
                {
                    entity.HasKey(e => new { e.ProductKey, e.OptionKey })
                        .HasName("PK_merchProduct2Option");

                    entity.ToTable("merchProduct2ProductOption");

                    entity.Property(e => e.ProductKey).HasColumnName("productKey");

                    entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.UseName)
                        .HasColumnName("useName")
                        .HasMaxLength(255);

                    entity.HasOne(d => d.OptionKeyNavigation)
                        .WithMany(p => p.MerchProduct2ProductOption)
                        .HasForeignKey(d => d.OptionKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProduct2Option_merchOption");

                    entity.HasOne(d => d.ProductKeyNavigation)
                        .WithMany(p => p.MerchProduct2ProductOption)
                        .HasForeignKey(d => d.ProductKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProduct2Option_merchProduct");
                });
        }
    }
}