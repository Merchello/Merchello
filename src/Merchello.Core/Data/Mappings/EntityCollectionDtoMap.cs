namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class EntityCollectionDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EntityCollectionDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchEntityCollection");

                    entity.ToTable("merchEntityCollection");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.EntityTfKey).HasColumnName("entityTfKey");

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.IsFilter).HasColumnName("isFilter");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.ParentKey).HasColumnName("parentKey");

                    entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                    entity.Property(e => e.SortOrder)
                        .HasColumnName("sortOrder")
                        .HasDefaultValueSql("'0'");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.ParentKeyNavigation)
                        .WithMany(p => p.InverseParentKeyNavigation)
                        .HasForeignKey(d => d.ParentKey)
                        .HasConstraintName("FK_merchEntityCollection_merchEntityCollection");
                });
        }
    }
}