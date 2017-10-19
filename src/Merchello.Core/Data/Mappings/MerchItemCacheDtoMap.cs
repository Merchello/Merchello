namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchItemCacheDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ItemCacheDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchItemCache");

                    entity.ToTable("merchItemCache");

                    entity.HasIndex(e => e.EntityKey)
                        .HasName("IX_merchItemCacheEntityKey");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.EntityKey).HasColumnName("entityKey");

                    entity.Property(e => e.ItemCacheTfKey).HasColumnName("itemCacheTfKey");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.VersionKey)
                        .HasColumnName("versionKey")
                        .HasDefaultValueSql("'newid()'");
                });
        }
    }
}