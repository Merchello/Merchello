namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class DetachedContentTypeDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DetachedContentTypeDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchDetachedContentType");

                    entity.ToTable("merchDetachedContentType");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.ContentTypeKey).HasColumnName("contentTypeKey");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Description)
                        .HasColumnName("description")
                        .HasMaxLength(1000);

                    entity.Property(e => e.EntityTfKey).HasColumnName("entityTfKey");

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasColumnName("name")
                        .HasMaxLength(255);

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");
                });
        }
    }
}