namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class AuditLogDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<AuditLogDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchAuditLog");

                    entity.ToTable("merchAuditLog");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.EntityKey).HasColumnName("entityKey");

                    entity.Property(e => e.EntityTfKey).HasColumnName("entityTfKey");

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.IsError).HasColumnName("isError");

                    entity.Property(e => e.Message)
                        .HasColumnName("message")
                        .HasColumnType("ntext");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Verbosity)
                        .HasColumnName("verbosity")
                        .HasDefaultValueSql("'0'");
                });

        }
    }
}