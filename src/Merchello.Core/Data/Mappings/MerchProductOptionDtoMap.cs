namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchProductOptionDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchProductOption>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchProductOption");

                    entity.ToTable("merchProductOption");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.DetachedContentTypeKey).HasColumnName("detachedContentTypeKey");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.Required)
                        .HasColumnName("required")
                        .HasDefaultValueSql("'0'");

                    entity.Property(e => e.Shared)
                        .HasColumnName("shared")
                        .HasDefaultValueSql("'0'");

                    entity.Property(e => e.UiOption)
                        .HasColumnName("uiOption")
                        .HasMaxLength(50);

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.DetachedContentTypeDtoKeyNavigation)
                        .WithMany(p => p.MerchProductOption)
                        .HasForeignKey(d => d.DetachedContentTypeKey)
                        .HasConstraintName("FK_merchProductOptionDetachedContent_merchProductOption");
                });
        }
    }
}