namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class AnonymousCustomerDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AnonymousCustomerDto>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchAnonymousCustomer");

                        entity.ToTable("merchAnonymousCustomer");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.ExtendedData).HasColumnName("extendedData").HasColumnType("ntext");

                        entity.Property(e => e.LastActivityDate)
                            .HasColumnName("lastActivityDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");
                    });
        }
    }
}