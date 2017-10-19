namespace Merchello.Core.Data.Mappings
{
    using Merchello.Core.Data.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;

    internal class MerchTaxMethodDtoMap : IEntityMap
    {
        public void Configure(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaxMethodDto>(
                entity =>
                    {
                        entity.HasKey(e => e.Pk).HasName("PK_merchTaxMethod");

                        entity.ToTable("merchTaxMethod");

                        entity.Property(e => e.Pk).HasColumnName("pk").HasDefaultValueSql("'newid()'");

                        entity.Property(e => e.CountryCode).IsRequired().HasColumnName("countryCode").HasMaxLength(255);

                        entity.Property(e => e.CreateDate)
                            .HasColumnName("createDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(255);

                        entity.Property(e => e.PercentageTaxRate)
                            .HasColumnName("percentageTaxRate")
                            .HasColumnType("numeric")
                            .HasDefaultValueSql("'0'");

                        entity.Property(e => e.ProductTaxMethod).HasColumnName("productTaxMethod");

                        entity.Property(e => e.ProviderKey).HasColumnName("providerKey");

                        entity.Property(e => e.ProvinceData).HasColumnName("provinceData").HasColumnType("ntext");

                        entity.Property(e => e.UpdateDate)
                            .HasColumnName("updateDate")
                            .HasColumnType("datetime")
                            .HasDefaultValueSql("getdate()");

                        entity.HasOne(d => d.ProviderKeyNavigation)
                            .WithMany(p => p.MerchTaxMethod)
                            .HasForeignKey(d => d.ProviderKey)
                            .OnDelete(DeleteBehavior.Restrict)
                            .HasConstraintName("FK_merchTaxMethod_merchGatewayProviderSettings");
                    });
        }
    }
}