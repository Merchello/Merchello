namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class CustomerAddressDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CustomerAddressDto>(entity =>
                {
                    entity.HasKey(e => e.Pk)
                        .HasName("PK_merchCustomerAddress");

                    entity.ToTable("merchCustomerAddress");

                    entity.Property(e => e.Pk)
                        .HasColumnName("pk")
                        .HasDefaultValueSql("'newid()'");

                    entity.Property(e => e.Address1)
                        .HasColumnName("address1")
                        .HasMaxLength(255);

                    entity.Property(e => e.Address2)
                        .HasColumnName("address2")
                        .HasMaxLength(255);

                    entity.Property(e => e.AddressTfKey).HasColumnName("addressTfKey");

                    entity.Property(e => e.Company)
                        .HasColumnName("company")
                        .HasMaxLength(255);

                    entity.Property(e => e.CountryCode)
                        .HasColumnName("countryCode")
                        .HasMaxLength(255);

                    entity.Property(e => e.CreateDate)
                        .HasColumnName("createDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.Property(e => e.CustomerKey).HasColumnName("customerKey");

                    entity.Property(e => e.FullName)
                        .HasColumnName("fullName")
                        .HasMaxLength(255);

                    entity.Property(e => e.IsDefault).HasColumnName("isDefault");

                    entity.Property(e => e.Label)
                        .HasColumnName("label")
                        .HasMaxLength(255);

                    entity.Property(e => e.Locality)
                        .HasColumnName("locality")
                        .HasMaxLength(255);

                    entity.Property(e => e.Phone)
                        .HasColumnName("phone")
                        .HasMaxLength(255);

                    entity.Property(e => e.PostalCode)
                        .HasColumnName("postalCode")
                        .HasMaxLength(255);

                    entity.Property(e => e.Region)
                        .HasColumnName("region")
                        .HasMaxLength(255);

                    entity.Property(e => e.UpdateDate)
                        .HasColumnName("updateDate")
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("getdate()");

                    entity.HasOne(d => d.CustomerDtoKeyNavigation)
                        .WithMany(p => p.MerchCustomerAddress)
                        .HasForeignKey(d => d.CustomerKey)
                        .OnDelete(DeleteBehavior.Restrict)
                        .HasConstraintName("FK_merchCustomerAddress_merchCustomer");
                });
        }
    }
}