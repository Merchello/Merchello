namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchOrderIndexDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderIndexDto>(entity =>
                {
                    entity.ToTable("merchOrderIndex");

                    entity.HasIndex(e => e.OrderKey)
                        .HasName("IX_merchOrderIndex")
                        .IsUnique();

                    entity.Property(e => e.Id).HasColumnName("id");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.OrderKey).HasColumnName("orderKey");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    //entity.HasOne(d => d.OrderKeyNavigation)
                    //    .WithOne(p => p.MerchOrderIndex)
                    //    .HasForeignKey<MerchOrderIndex>(d => d.OrderKey)
                    //    .OnDelete(DeleteBehavior.Restrict)
                    //    .HasConstraintName("FK_merchOrderIndex_merchOrder");
                });
        }
    }
}