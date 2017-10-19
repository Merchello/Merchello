namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class MerchNoteDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<NoteDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchNote");

                    entity.ToTable("merchNote");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.Author)
                        .HasColumnName("author")
                        .HasMaxLength(255);

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.EntityKey).HasColumnName("entityKey");

                    entity.Property(e => e.EntityTfKey).HasColumnName("entityTfKey");

                    entity.Property(e => e.InternalOnly).HasColumnName("internalOnly");

                    entity.Property(e => e.Message)
                        .HasColumnName("message")
                        .HasColumnType("ntext");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");
                });
        }
    }
}