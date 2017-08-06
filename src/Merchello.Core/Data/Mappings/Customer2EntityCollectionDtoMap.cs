namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class Customer2EntityCollectionDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer2EntityCollectionDto>(entity =>
                {
                    entity.HasKey(e => new { e.CustomerKey, e.EntityCollectionKey })
                        .HasName("PK_merchCustomer2EntityCollection");

                    entity.ToTable("merchCustomer2EntityCollection");

                    entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                    entity.Property(e => e.EntityCollectionKey).HasColumnName("entityCollectionKey");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.CustomerDtoKeyNavigation)
                        .WithMany(p => p.MerchCustomer2EntityCollection)
                        .HasForeignKey(d => d.CustomerKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchCustomer2EntityCollection_merchInvoice");

                    entity.HasOne(d => d.EntityCollectionDtoKeyNavigation)
                        .WithMany(p => p.MerchCustomer2EntityCollection)
                        .HasForeignKey(d => d.EntityCollectionKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchCustomer2EntityCollection_merchEntityCollection");
                });
        }
    }
}