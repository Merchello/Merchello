namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;

    internal class CustomerDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchCustomer");

                    entity.ToTable("merchCustomer");

                    entity.HasIndex(e => e.LoginName)
                        .HasName("IX_merchCustomerLoginName")
                        .IsUnique();

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.Email)
                        .IsRequired()
                        .HasColumnName("email")
                        .HasMaxLength(255);

                    entity.Property(e => e.ExtendedData)
                        .HasColumnName("extendedData")
                        .HasColumnType("ntext");

                    entity.Property(e => e.FirstName)
                        .IsRequired()
                        .HasColumnName("firstName")
                        .HasMaxLength(255);

                    entity.Property(e => e.LastActivityDate)
                        .HasColumnName("lastActivityDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.LastName)
                        .IsRequired()
                        .HasColumnName("lastName")
                        .HasMaxLength(255);

                    entity.Property(e => e.LoginName)
                        .IsRequired()
                        .HasColumnName("loginName")
                        .HasMaxLength(255);

                    entity.Property(e => e.Notes)
                        .HasColumnName("notes")
                        .HasColumnType("ntext");

                    entity.Property(e => e.TaxExempt).HasColumnName("taxExempt");

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");
                });
        }
    }
}