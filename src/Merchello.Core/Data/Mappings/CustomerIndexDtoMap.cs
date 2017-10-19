namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class CustomerIndexDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerIndexDto>(entity =>
                {
                    entity.ToTable("merchCustomerIndex");

                    entity.HasIndex(e => e.CustomerKey)
                        .HasName("IX_merchCustomerIndex")
                        .IsUnique();

                    entity.Property(e => e.Id).HasColumnName("id");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    //entity.HasOne(d => d.CustomerDtoKeyNavigation)
                    //    .WithOne(p => p.CustomerIndexDto)
                    //    .HasForeignKey<CustomerIndexDto>(d => d.CustomerKey)
                    //    .OnDelete(DeleteBehavior.Restrict)
                    //    .HasConstraintName("FK_merchCustomerIndex_merchCustomer");
                });
        }
    }
}