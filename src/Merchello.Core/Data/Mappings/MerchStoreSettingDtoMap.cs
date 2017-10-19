namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchStoreSettingDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StoreSettingDto>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchStoreSetting");

                        entity.ToTable("merchStoreSetting");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Name).IsRequired().HasColumnName("name").HasMaxLength(255);

                        entity.Property(e => e.TypeName).IsRequired().HasColumnName("typeName").HasMaxLength(255);

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Value).IsRequired().HasColumnName("value").HasMaxLength(255);
                    });
        }
    }
}