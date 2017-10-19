namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchProductAttributeDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MerchProductAttribute>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchProductAttribute");

                    entity.ToTable("merchProductAttribute");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.DetachedContentValues)
                        .HasColumnName("detachedContentValues")
                        .HasColumnType("ntext");

                    entity.Property(e => e.IsDefaultChoice)
                        .HasColumnName("isDefaultChoice")
                        .HasDefaultValueSql("'0'");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.OptionKey).HasColumnName("optionKey");

                    entity.Property(e => e.Sku)
                        .IsRequired()
                        .HasColumnName("sku")
                        .HasMaxLength(255);

                    entity.Property(e => e.SortOrder).HasColumnName("sortOrder");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.OptionKeyNavigation)
                        .WithMany(p => p.MerchProductAttribute)
                        .HasForeignKey(d => d.OptionKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchProductAttribute_merchOption");
                });
        }
    }
}